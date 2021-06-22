using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.EntityFrameworkCore;
using RakDotNet;
using RakDotNet.IO;
using RakDotNet.TcpUdp;
using Sentry;
using StackExchange.Redis;
using Uchu.Api;
using Uchu.Api.Models;
using Uchu.Core.Api;
using Uchu.Core.Config;
using Uchu.Core.IO;
using Uchu.Core.Providers;
using Uchu.Core.Resources;
using Uchu.Sso;

namespace Uchu.Core
{
    using HandlerMap = Dictionary<RemoteConnectionType, Dictionary<uint, Handler>>;

    using CommandHandleMap = Dictionary<char, Dictionary<string, CommandHandler>>;

    /// <summary>
    /// Main server class that handles incoming connections and packets
    /// </summary>
    public class UchuServer
    {
        /// <summary>
        /// Map that contains all packet handlers
        /// </summary>
        protected HandlerMap HandlerMap { get; }

        /// <summary>
        /// Map that contains all command handlers
        /// </summary>
        protected CommandHandleMap CommandHandleMap { get; }

        /// <summary>
        /// The underlying TcpUdp server
        /// </summary>
        public IRakServer RakNetServer { get; private set; }

        /// <summary>
        /// Cache for the server
        /// </summary>
        public ISessionCache SessionCache { get; private set; }

        /// <summary>
        /// Resource files for the server
        /// </summary>
        public IFileResources Resources { get; private set; }

        /// <summary>
        /// Configuration file used to setup the server
        /// </summary>
        public static UchuConfiguration Config { get; private set; }
        
        /// <summary>
        /// The service used for Single Sign On (SSO)
        /// </summary>
        public SsoService SsoService { get; private set; }
        
        /// <summary>
        /// GUID of this server
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// Port of the API server that belongs to this server
        /// </summary>
        public int ApiPort { get; private set; }
        
        /// <summary>
        /// Port of this server
        /// </summary>
        public int Port { get; private set; }
        
        /// <summary>
        /// The API port of the master server
        /// </summary>
        public int MasterApi { get; private set; }

        /// <summary>
        /// The path to the master server
        /// </summary>
        public string MasterPath { get; private set; }
        
        /// <summary>
        /// The API of this server (handles commands and such)
        /// </summary>
        public ApiManager Api { get; private set; }

        /// <summary>
        /// Event called whenever a game message is received from the client
        /// </summary>
        public event Func<long, ushort, BitReader, IRakConnection, Task> GameMessageReceived;

        /// <summary>
        /// Event that is called after the server has stopped
        /// </summary>
        public event Action ServerStopped;

        /// <summary>
        /// Whether the server is running or not
        /// </summary>
        protected bool Running { get; private set; }

        /// <summary>
        /// Optional certificate for SSL
        /// </summary>
        protected X509Certificate Certificate { get; set; }
        
        /// <summary>
        /// The amount of heart beats the world server should send to the master server to retain its healthy status. If
        /// the world server can't keep up with this amount it risks being killed by the master server
        /// </summary>
        public int HeartBeatsPerInterval { get; set; }
        
        /// <summary>
        /// The total time in which <see cref="HeartBeatsPerInterval"/> heart beats should have been sent to retain the
        /// healthy status
        /// </summary>
        public int HeartBeatIntervalInMinutes { get; set; }
        
        /// <summary>
        /// The interval in milliseconds at which heart beats should be sent
        /// </summary>
        /// <remarks>
        /// Returns essentially one heart beat more than absolutely necessary for a healthy status to ensure that small
        /// hick ups and lag do not interfere with overall server health 
        /// </remarks>
        public float HeartBeatInterval => (float) HeartBeatIntervalInMinutes * 60000 / (HeartBeatsPerInterval + 1);

        /// <summary>
        /// The host on which the server is running, used for example for API callbacks
        /// </summary>
        public string Host => !string.IsNullOrWhiteSpace(Config.Networking.Hostname) ? Config.Networking.Hostname : "localhost";
        
        /// <summary>
        /// Generates a command help message based on certain parameters 
        /// </summary>
        /// <param name="prefix">The prefix to use for each command (for example "/")</param>
        /// <param name="group">The group to get commands from</param>
        /// <param name="level">The gamemaster level to determine which commands to show</param>
        /// <returns>A string that represents the help message</returns>
        private static string GenerateCommandHelpMessage(char prefix, Dictionary<string, CommandHandler> group,
            GameMasterLevel level)
        {
            var help = new StringBuilder();
            help.AppendLine("Available commands:");

            foreach (var handlerInfo in group.Values.Where(handlerInfo => level >= handlerInfo.GameMasterLevel))
            {
                help.AppendLine($"{prefix}{handlerInfo.Signature} \n" +
                                $"      {handlerInfo.Help}");
            }

            return help.ToString();
        }

        public UchuServer(Guid id)
        {
            Id = id;
            
            HandlerMap = new HandlerMap();
            
            CommandHandleMap = new CommandHandleMap();
        }

        /// <summary>
        /// Configures the server by parsing config files and setting up certificates.
        /// </summary>
        /// <param name="configFile">The config file to use as configuration</param>
        /// <exception cref="ArgumentException">If the provided config file can't be found</exception>
        public virtual async Task ConfigureAsync(string configFile)
        {
            if (configFile == null)
                throw new ArgumentNullException(nameof(configFile), 
                    ResourceStrings.Server_ConfigureAsync_ConfigFileNullException);
            
            MasterPath = Path.GetDirectoryName(configFile);
            var serializer = new XmlSerializer(typeof(UchuConfiguration));

            if (!File.Exists(configFile))
            {
                throw new ArgumentException($"{configFile} config file does not exist.");
            }

            await using (var fs = File.OpenRead(configFile))
            {
                using (var xmlReader = XmlReader.Create(fs))
                {
                    LogQueue.Config = Config = (UchuConfiguration) serializer.Deserialize(xmlReader);
                    UchuContextBase.Config = Config;
                }
            }

            await SetupApiAsync().ConfigureAwait(false);
            if (!string.IsNullOrWhiteSpace(Config.ResourcesConfiguration?.GameResourceFolder))
            {
                Resources = new LocalResources(Config);
            }
            
            // Setup the RakNet server and possible certificate
            var certificateFilePath = Path.Combine(MasterPath, Config.Networking.Certificate);
            if (Config.Networking?.Certificate != default && File.Exists(certificateFilePath))
            {
                var cert = new X509Certificate2(certificateFilePath);
                Logger.Information($"PRIVATE KEY: {cert.HasPrivateKey} {cert.PrivateKey}");
                Certificate = cert;
                RakNetServer = new TcpUdpServer(Port, "3.25 ND1", Certificate, 150);
            }
            else
            {
                RakNetServer = new TcpUdpServer(Port, "3.25 ND1");
            }

            SsoService = new SsoService(Config.SsoConfig?.Domain ?? "");

            // Try to connect to Redis, otherwise fallback to DB caching
            if (Config.CacheConfig.UseService)
            {
                try
                {
                    SessionCache = new RedisSessionCache(Config.CacheConfig);
                    Logger.Information($"Established Redis connection at {Config.CacheConfig.Host}:{Config.CacheConfig.Port}");
                }
                catch (RedisConnectionException)
                {
                    Logger.Error("Failed to establish Redis connection, falling back to database.");
                    SessionCache = new DatabaseCache();
                }
            }
            else
            {
                Logger.Information("Caching service is disabled, falling back to database.");
                SessionCache = new DatabaseCache();
            }

            HeartBeatsPerInterval = Config.Networking.WorldServerHeartBeatsPerInterval;
            HeartBeatIntervalInMinutes = Config.Networking.WorldServerHeartBeatIntervalInMinutes;

            Logger.Information($"Server {Id} configured on port: {Port}");
        }

        /// <summary>
        /// Sets up the API that handles all server commands
        /// </summary>
        /// <exception cref="Exception">An exception if the setup of the API failed</exception>
        public async Task SetupApiAsync()
        {
            Api = new ApiManager(Config.ApiConfig.Protocol, Config.ApiConfig.Domain);

            var instance = await Api.RunCommandAsync<InstanceInfoResponse>(
                Config.ApiConfig.Port, $"instance/target?i={Id}"
            ).ConfigureAwait(false);

            if (!instance.Success)
            {
                Logger.Error(instance.FailedReason);
                throw new Exception(instance.FailedReason);
            }

            Port = instance.Info.Port;
            ApiPort = instance.Info.ApiPort;
            MasterApi = instance.Info.MasterApi;

            Api.RegisterCommandCollection<InstanceCommands>(this);
        }

        /// <summary>
        /// Starts the server from the provided assembly
        /// </summary>
        /// <param name="assembly">The Uchu instance to start</param>
        /// <param name="acceptConsoleCommands">Whether to handle console commands or not</param>
        public async Task StartAsync(Assembly assembly, bool acceptConsoleCommands = false)
        {
            using (SentrySdk.Init(Config.SentryDsn))
            {
                RegisterDefaultAssemblies(assembly);
                if (acceptConsoleCommands)
                {
                    StartConsole();
                }
                
                Running = true;

                var server = RakNetServer.RunAsync();
                var api = Api.StartAsync(ApiPort);

                var tasks = new[]
                {
                    server,
                    api
                };

                await Task.WhenAny(tasks).ConfigureAwait(false);
                
                Console.WriteLine($"EXIT: {server.Status} | {api.Status}");

                await StopAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Starts the console making the server accept console commands
        /// </summary>
        private void StartConsole()
        {
            Task.Run(async () =>
            {
                Logger.Information("Ready to accept console command...");

                while (Running)
                {
                    var command = Console.ReadLine();

                    if (string.IsNullOrWhiteSpace(command)) continue;

                    Console.WriteLine(await HandleCommandAsync(command, null, GameMasterLevel.Console)
                        .ConfigureAwait(false));
                }
            });
        }

        /// <summary>
        /// Registers a server assembly along with the assembly to load
        /// </summary>
        /// <param name="assembly">The assembly to load</param>
        private void RegisterDefaultAssemblies(Assembly assembly)
        {
            Logger.Information("Registering assemblies...");

            RegisterAssembly(typeof(UchuServer).Assembly);
            RegisterAssembly(assembly);

            RakNetServer.MessageReceived += HandlePacketAsync;
        }

        /// <summary>
        /// Shuts down the server
        /// </summary>
        public Task StopAsync()
        {
            Console.WriteLine(ResourceStrings.Server_StopAsync_Log);

            Running = false;
            ServerStopped?.Invoke();
            Certificate?.Dispose();
            Api?.Close();
            
            return RakNetServer.ShutdownAsync();
        }

        /// <summary>
        /// Registers an assembly by fetching all handlers from it and storing them in the server
        /// </summary>
        /// <param name="assembly">The assembly to register</param>
        /// <exception cref="ArgumentNullException">If the assembly is empty</exception>
        public virtual void RegisterAssembly(Assembly assembly)
        {
            if (assembly == default)
                throw new ArgumentNullException(nameof(assembly));

            var groups = assembly.GetTypes().Where(c => c.IsSubclassOf(typeof(HandlerGroup)));
            foreach (var group in groups)
            {
                var instance = (HandlerGroup) Activator.CreateInstance(group);
                instance.SetServer(this);

                // Get all packet handlers and add them to the handler map
                foreach (var method in group.GetMethods().Where(m => !m.IsAbstract))
                {
                    if (method.GetCustomAttribute<PacketHandlerAttribute>() is {} packetHandlerAttribute)
                    {
                        RegisterPacketHandler(method, packetHandlerAttribute, instance);
                    }
                    else if (method.GetCustomAttribute<CommandHandlerAttribute>() is {} commandHandlerAttribute)
                    {
                        RegisterCommandHandler(method, commandHandlerAttribute, instance);
                    }
                }
            }
        }

        /// <summary>
        /// Adds a packet handler to the Handler Map
        /// </summary>
        /// <param name="method">The signature of the method to add</param>
        /// <param name="attribute">The type of packet handler to add</param>
        /// <param name="instance">The handler group that manages this handler</param>
        private void RegisterPacketHandler(MethodInfo method, PacketHandlerAttribute attribute, HandlerGroup instance)
        {
            var parameters = method.GetParameters();
            if (parameters.Length == 0 || !typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType))
                return;
            
            var packet = (IPacket) Activator.CreateInstance(parameters[0].ParameterType);
            if (packet == null)
                return;

            var remoteConnectionType = attribute.RemoteConnectionType ?? packet.RemoteConnectionType;
            var packetId = attribute.PacketId ?? packet.PacketId;

            if (!HandlerMap.ContainsKey(remoteConnectionType))
                HandlerMap[remoteConnectionType] = new Dictionary<uint, Handler>();

            var handlers = HandlerMap[remoteConnectionType];

            Logger.Debug(!handlers.ContainsKey(packetId)
                ? $"Registered handler for packet {packet}"
                : $"Handler for packet {packet} overwritten"
            );

            handlers[packetId] = new Handler(instance, method, parameters[0].ParameterType);
        }

        /// <summary>
        /// Registers a command in the handler map
        /// </summary>
        /// <param name="method">The signature of the command</param>
        /// <param name="commandAttribute">The type of command handler attribute</param>
        /// <param name="instance">The handler group that manages this handler</param>
        private void RegisterCommandHandler(MethodInfo method, CommandHandlerAttribute commandAttribute,
            HandlerGroup instance)
        {
            if (!CommandHandleMap.ContainsKey(commandAttribute.Prefix))
                CommandHandleMap[commandAttribute.Prefix] = new Dictionary<string, CommandHandler>();

            CommandHandleMap[commandAttribute.Prefix][commandAttribute.Signature] = new CommandHandler
            {
                Group = instance,
                Info = method,
                GameMasterLevel = commandAttribute.GameMasterLevel,
                Help = commandAttribute.Help,
                Signature = commandAttribute.Signature,
                ConsoleCommand = method.GetParameters().Length != 2
            };
        }

        /// <summary>
        /// Handles any incoming packet from an endpoint
        /// </summary>
        /// <param name="endPoint">The IP that the packet originated from</param>
        /// <param name="data">Binary data of the packet</param>
        /// <param name="reliability">Reliability of the packet</param>
        /// <exception cref="ArgumentOutOfRangeException">If the packet is not a valid user packet</exception>
        [SuppressMessage("ReSharper", "CA1031")]
        public async Task HandlePacketAsync(IPEndPoint endPoint, byte[] data, Reliability reliability)
        {
            // Connection can be null when packets are sent over an invalid port
            var connection = RakNetServer.GetConnection(endPoint);
            if (connection == null)
            {
                Logger.Warning($"Received packet from client that is not logged in ({endPoint}), ignoring.");
                return;
            }

            await using var stream = new MemoryStream(data);
            using var reader = new BitReader(stream);
            
            var header = new PacketHeader();
            reader.Read(header);

            if (header.MessageId != MessageIdentifier.UserPacketEnum)
                throw new ArgumentOutOfRangeException($"Packet is not {nameof(MessageIdentifier.UserPacketEnum)}");

            // Game Message
            if (header.PacketId == 0x05)
            {
                var objectId = reader.Read<long>();
                var messageId = reader.Read<ushort>();

                try
                {
                    if (GameMessageReceived != null)
                    {
                        await GameMessageReceived(objectId, messageId, reader, connection).ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    Logger.Error($"Error when handling GM: {e}");
                }
            }
            else
            {
                // Regular Packet
                if (!HandlerMap.TryGetValue(header.RemoteConnectionType, out var temp) ||
                    !temp.TryGetValue(header.PacketId, out var handler))
                {
                    Logger.Warning($"No handler registered for Packet ({header.RemoteConnectionType}:0x{header.PacketId:x})!");
                    return;
                }

                Logger.Debug($"Received {handler.PacketType.FullName}");
                reader.BaseStream.Position = 8;

                try
                {
                    if (handler.PacketType.IsValueType)
                    {
                        // Struct-based packet.
                        var packet = StructPacketParser.ReadPacket(handler.PacketType, reader.BaseStream, null);
                        await InvokeHandlerAsync(handler, packet, connection).ConfigureAwait(false);
                    }
                    else
                    {
                        // IPacket-based packet.
                        var packet = handler.NewPacket();
                        reader.Read(packet);
                        await InvokeHandlerAsync(handler, packet, connection).ConfigureAwait(false);
                    }
                }
                catch (Exception e)
                {
                    SentrySdk.CaptureException(e);
                    Logger.Error($"Error when handling packet: {e}");
                }
            }
        }

        /// <summary>
        /// Handles an incoming command
        /// </summary>
        /// <param name="command">The fulltext command to execute</param>
        /// <param name="author">Author of the command</param>
        /// <param name="gameMasterLevel">The game master level of the author</param>
        public async Task<string> HandleCommandAsync(string command, object author, GameMasterLevel gameMasterLevel)
        {
            // Check if the command exists and is valid
            if (command == default) return default;
            var prefix = command.FirstOrDefault();
            if (!CommandHandleMap.TryGetValue(prefix, out var group)) return default;

            // Remove the slash
            command = command.Remove(0, 1);
            Logger.Information($"EXEC: {command}");
            
            // Split arguments at spaces, preserving "groups enclosed in quotes" as a single argument.
            var arguments = command.Split('"')
                .Select((element, index) => index % 2 == 0 // If number of " seen is even, it's not between quotes
                    ? element.Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split item at spaces
                    : new string[] {element}) // Keep the entire item
                .SelectMany(args => args)
                .ToList();

            // If the command can't be found, display help message
            if (!group.TryGetValue(arguments[0].ToLower(CultureInfo.CurrentCulture), out var handler))
            {
                if (arguments[0].ToLower(CultureInfo.CurrentCulture) == "help")
                    return GenerateCommandHelpMessage(prefix, group, gameMasterLevel);

                return "Unknown command. Type /help for a list of commands.";
            }

            if (gameMasterLevel < handler.GameMasterLevel)
                return "You don't have permission to run this command";

            arguments.RemoveAt(0);

            // Determine how to call the task handler based on the amount of arguments
            var paramLength = handler.Info.GetParameters().Length;
            var returnValue = paramLength switch
            {
                0 => handler.Info.Invoke(handler.Group, Array.Empty<object>()),
                1 => handler.Info.Invoke(handler.Group, new object[] {arguments.ToArray()}),
                _ => handler.Info.Invoke(handler.Group, new[] {arguments.ToArray(), author})
            };
            switch (returnValue)
            {
                case string s:
                    return s;
                case Task<string> s:
                    return await s.ConfigureAwait(false);
                case Task t:
                    await t.ConfigureAwait(false);
                    break;
            }

            return "";
        }

        /// <summary>
        /// Validates a user before allowing a connection
        /// </summary>
        /// <param name="connection">The connection endpoint</param>
        /// <param name="username">The username to validate</param>
        /// <param name="key">The session key of the user</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public async Task<bool> ValidateUserAsync(IRakConnection connection, string username, string key)
        {
            if (connection == null)
            {
                Logger.Warning($"Received client login from invalid connection ({username}), ignoring.");
                return false;
            }
            
            var sso = await SsoService.VerifyAsync(username, key).ConfigureAwait(false);
            var local = SessionCache.IsKey(key);
            
            if (!sso && !local)
            {
                Logger.Warning($"{connection} attempted to connect with an invalid session key");
                await connection.CloseAsync().ConfigureAwait(false);
                return false;
            }

            // Handle Sso users
            if (sso && !await HandleSsoUser(connection, username, key).ConfigureAwait(false))
                return false;

            SessionCache.RegisterKey(connection.EndPoint, key);
            return true;
        }

        /// <summary>
        /// Validates an SSO user by optionally registering them and ensuring they're not banned
        /// </summary>
        /// <param name="connection">The connection of the SSO user (closed if banned)</param>
        /// <param name="username">The username of the SSO user</param>
        /// <param name="key">The SSO key</param>
        /// <returns><c>true</c> if the user is valid, <c>false</c> otherwise</returns>
        private async Task<bool> HandleSsoUser(IRakConnection connection, string username, string key)
        {
            await RegisterSsoUserIfNewForUsername(username).ConfigureAwait(false);
            return await ValidateSsoUserForUsername(connection, username, key)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Registers a SSO user from a username if they don't exist yet
        /// </summary>
        /// <param name="username">Username to find a user for</param>
        /// <returns>The user</returns>
        [SuppressMessage("ReSharper", "CA2000")]
        private static async Task<User> RegisterSsoUserIfNewForUsername(string username)
        {
            await using var ctx = new UchuContext();
            var user = await ctx.Users.Where(u => u.Sso).FirstOrDefaultAsync(
                u => u.Username == username
            ).ConfigureAwait(false);

            // If the user doesn't exist, add them
            if (user == default)
            {
                user = new User
                {
                    Username = username,
                    Sso = true
                };
                await ctx.Users.AddAsync(user).ConfigureAwait(false);
            }
            
            await ctx.SaveChangesAsync().ConfigureAwait(false);
            return user;
        }
        
        /// <summary>
        /// Validates if a SSO user is valid by ensuring they exist and aren't banned
        /// </summary>
        /// <param name="connection">The connection to close if the user is banned</param>
        /// <param name="username">The username of the SSO user</param>
        /// <returns><c>true</c> if the user is valid, <c>false</c> otherwise.</returns>
        [SuppressMessage("ReSharper", "CA2000")]
        private async Task<bool> ValidateSsoUserForUsername(IRakConnection connection, string username, string key)
        {
            await using var ctx = new UchuContext();
            {
                var user = await ctx.Users.Where(u => u.Sso).FirstOrDefaultAsync(
                    u => u.Username == username
                ).ConfigureAwait(false);
                    
                if (user == default) return false;
                if (user.Banned || !string.IsNullOrWhiteSpace(user.CustomLockout))
                {
                    await connection.CloseAsync().ConfigureAwait(false);
                    return false;
                }
                    
                if (!SessionCache.IsKey(key))
                {
                    SessionCache.CreateSession(user.Id, key);
                }
            }
            
            return true;
        }

        /// <summary>
        /// Invokes a paket handler
        /// </summary>
        /// <param name="handler">The handler to invoke</param>
        /// <param name="endPoint">The endpoint to send a response to</param>
        private static async Task InvokeHandlerAsync(Handler handler, object packet, IRakConnection endPoint)
        {
            var task = handler.Info.ReturnType == typeof(Task);
            
            var parameters = new object[] {packet, endPoint};
            var res = handler.Info.Invoke(handler.Group, parameters);

            if (task && res != null)
                await ((Task) res).ConfigureAwait(false);
            if (res == null)
                Logger.Warning($"Handler {handler.GetType().FullName} returned null for {endPoint}.");
        }
    }
}