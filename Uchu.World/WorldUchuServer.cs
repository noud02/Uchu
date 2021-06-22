using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Timers;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Python;
using Uchu.World.Api;
using Uchu.World.Client;
using Uchu.World.Social;

namespace Uchu.World
{
    using GameMessageHandlerMap = Dictionary<GameMessageId, Handler>;

    public class WorldUchuServer : UchuServer
    {
        private readonly GameMessageHandlerMap _gameMessageHandlerMap;

        private ZoneId ZoneId { get; set; }

        public List<Zone> Zones { get; }

        public uint MaxPlayerCount { get; }
        
        public ZoneParser ZoneParser { get; private set; }
        
        public Whitelist Whitelist { get; private set; }

        /// <summary>
        /// Sends a heart beat to the master server, indicating the health of the server
        /// </summary>
        public async Task SendHeartBeat()
        {
            await Api.RunCommandAsync<BaseResponse>(MasterApi, $"instance/heartbeat?instance={Id.ToString()}")
                .ConfigureAwait(false);
        }

        public WorldUchuServer(Guid id) : base(id)
        {
            Zones = new List<Zone>();

            MaxPlayerCount = 20; // TODO: Set
            
            _gameMessageHandlerMap = new GameMessageHandlerMap();
        }

        public override async Task ConfigureAsync(string configFile)
        {
            Logger.Information($"Created WorldServer on PID {Process.GetCurrentProcess().Id.ToString()}");
            await base.ConfigureAsync(configFile);

            ZoneParser = new ZoneParser(Resources);
            Whitelist = new Whitelist(Resources);

            await Whitelist.LoadDefaultWhitelist();

            GameMessageReceived += HandleGameMessageAsync;
            ServerStopped += () =>
            {
                foreach (var zone in Zones)
                {
                    Object.Destroy(zone);
                }
            };

            var instance = await Api.RunCommandAsync<InstanceInfoResponse>(
                Config.ApiConfig.Port, $"instance/target?i={Id}"
            ).ConfigureAwait(false);

            ZoneId = (ZoneId) instance.Info.Zones.First();

            var info = await Api.RunCommandAsync<InstanceInfoResponse>(MasterApi, $"instance/target?i={Id}");

            Api.RegisterCommandCollection<WorldCommands>(this);

            ManagedScriptEngine.AdditionalPaths = Config.ManagedScriptSources.Paths.ToArray();

            _ = Task.Run(async () =>
            {
                Logger.Information("Loading CDClient cache");
                await ClientCache.LoadAsync();
            });

            _ = Task.Run(async () =>
            {
                Logger.Information($"Setting up zones for world server {Id}");
                foreach (var zone in info.Info.Zones)
                {
                    await ZoneParser.LoadZoneDataAsync(zone);
                    await LoadZone(zone);
                }
            });
        }

        private async Task LoadZone(int zoneId)
        {
            if (ZoneParser.Zones == default)
                await ZoneParser.LoadZoneDataAsync(zoneId);

            Logger.Information($"Starting {zoneId}");

            if (ZoneParser?.Zones == default || !ZoneParser.Zones.TryGetValue(zoneId, out var info))
            {
                throw new Exception($"Failed to find info for {(ZoneId) zoneId}");
            }

            // Ensure the correct Zone ID is used later on, this differs from the value in the .luz for eg. Moonbase and Deep Freeze
            info.LuzFile.WorldId = (ZoneId) zoneId;

            // TODO instance / clone
            var zone = new Zone(info, this, 0, 0);
            Zones.Add(zone);
            
            await zone.InitializeAsync();
        }

        public async Task<Zone> GetZoneAsync(ZoneId zoneId)
        {
            if (ZoneId == zoneId)
            {
                // Wait for zone to load, TODO: Use event here
                while (Running)
                {
                    var zone = Zones.FirstOrDefault(z => z.ZoneId == zoneId);

                    if (zone?.Loaded ?? false)
                    {
                        return zone;
                    }
                    
                    await Task.Delay(1000);
                }
            }
            
            Logger.Error($"{zoneId} is not in the Zone Table for this server.");
            return default;
        }

        /// <summary>
        /// Registers all packet handlers in an assembly
        /// </summary>
        /// <param name="assembly">The assembly to register</param>
        public override void RegisterAssembly(Assembly assembly)
        {
            var groups = assembly.GetTypes().Where(c => c.IsSubclassOf(typeof(HandlerGroup)));

            // Find all packet handler groups
            foreach (var group in groups)
            {
                var instance = (HandlerGroup) Activator.CreateInstance(group);
                instance.SetServer(this);
                
                foreach (var method in group.GetMethods().Where(m => !m.IsStatic && !m.IsAbstract))
                {
                    var attr = method.GetCustomAttribute<PacketHandlerAttribute>();
                    if (attr != null)
                    {
                        // Packet handlers and game messages
                        var parameters = method.GetParameters();
                        if (parameters.Length == 0)
                            continue;

                        if (typeof(IPacket).IsAssignableFrom(parameters[0].ParameterType))
                        {
                            // IPacket (object)-based handler.
                            var packet = (IPacket) Activator.CreateInstance(parameters[0].ParameterType);
                            if (packet == null)
                                continue;
                            
                            var remoteConnectionType = attr.RemoteConnectionType ?? packet.RemoteConnectionType;
                            var packetId = attr.PacketId ?? packet.PacketId;

                            if (!HandlerMap.ContainsKey(remoteConnectionType))
                                HandlerMap[remoteConnectionType] = new Dictionary<uint, Handler>();

                            var handlers = HandlerMap[remoteConnectionType];

                            Logger.Debug(!handlers.ContainsKey(packetId)
                                ? $"Registered handler for packet {packet}"
                                : $"Handler for packet {packet} overwritten");

                            handlers[packetId] = new Handler(instance, method, parameters[0].ParameterType);
                        }
                        else if (parameters[0].ParameterType.IsValueType)
                        {
                            // Struct-based handler.
                            var packet = Activator.CreateInstance(parameters[0].ParameterType);
                            if (packet == null)
                                continue;
                            
                            var gameMessageIdProperty = parameters[0].ParameterType.GetProperty("GameMessageId");
                            if (gameMessageIdProperty != null)
                            {
                                if (!(gameMessageIdProperty.GetValue(packet) is GameMessageId gameMessageId))
                                    continue;
                                _gameMessageHandlerMap.Add(gameMessageId, new Handler(instance, method, 
                                    parameters[0].ParameterType));
                            }
                            else
                            {
                                var packetStruct = parameters[0].ParameterType.GetCustomAttribute<PacketStruct>(true);
                                if (packetStruct == null)
                                    continue;
                                
                                var remoteConnectionType = attr.RemoteConnectionType ?? packetStruct.RemoteConnectionType;
                                var packetId = attr.PacketId ?? packetStruct.PacketId;

                                if (!HandlerMap.ContainsKey(remoteConnectionType))
                                    HandlerMap[remoteConnectionType] = new Dictionary<uint, Handler>();

                                var handlers = HandlerMap[remoteConnectionType];

                                Logger.Debug(!handlers.ContainsKey(packetId)
                                    ? $"Registered handler for packet {packet}"
                                    : $"Handler for packet {packet} overwritten");

                                handlers[packetId] = new Handler(instance, method, parameters[0].ParameterType);
                            }
                        }
                    }
                    else
                    {
                        // Command handlers
                        var cmdAttr = method.GetCustomAttribute<CommandHandlerAttribute>();
                        if (cmdAttr == null)
                            continue;

                        if (!CommandHandleMap.ContainsKey(cmdAttr.Prefix))
                            CommandHandleMap[cmdAttr.Prefix] = new Dictionary<string, CommandHandler>();

                        CommandHandleMap[cmdAttr.Prefix][cmdAttr.Signature] = new CommandHandler
                        {
                            Group = instance,
                            Info = method,
                            GameMasterLevel = cmdAttr.GameMasterLevel,
                            Help = cmdAttr.Help,
                            Signature = cmdAttr.Signature,
                            ConsoleCommand = method.GetParameters().Length != 2
                        };
                    }
                }
            }
        }

        /// <summary>
        /// Handles a game message received by the server
        /// </summary>
        /// <param name="objectId">The id of the object that sent the message</param>
        /// <param name="messageId">The ID of the message that is received</param>
        /// <param name="reader">Bit reader to read the stream of the rest of the game message</param>
        /// <param name="connection">Connection used by the client</param>
        private async Task HandleGameMessageAsync(long objectId, ushort messageId, BitReader reader, IRakConnection connection)
        {
            if (!_gameMessageHandlerMap.TryGetValue((GameMessageId) messageId, out var messageHandler))
            {
                Logger.Warning(Enum.IsDefined(typeof(GameMessageId), messageId)
                    ? $"No handler registered for GameMessage: {(GameMessageId) messageId}!"
                    : $"Undocumented GameMessage: 0x{messageId:x8}!"
                );
                return;
            }

            Logger.Debug($"Received {(GameMessageId)messageId}");

            // Check if this message came from a logged in player
            var player = Zones.SelectMany(z => z.Players).FirstOrDefault(p => p.Connection.Equals(connection));
            if (player?.Zone == default)
            {
                Logger.Error($"{connection} is not logged in but sent a GameMessage.");
                return;
            }

            // Find the owner of this message
            var associate = player.Zone.GameObjects.FirstOrDefault(o => o.Id == objectId);
            if (associate == default)
            {
                Logger.Error($"{objectId} is not a valid object in {connection}'s zone.");
                return;
            }

            // Handle a struct-based packet.
            var gameMessage = StructPacketParser.ReadPacket(messageHandler.PacketType, reader.BaseStream, new Dictionary<string, object>()
            {
                {"Zone", player.Zone}
            });
            await InvokeHandlerAsync(messageHandler, gameMessage, player);
        }

        /// <summary>
        /// Invokes the handler of a packet, causing the packet to be executed
        /// </summary>
        /// <param name="handler">The packet handler to call</param>
        /// <param name="packet">The packet to pass to the handler</param>
        /// <param name="player">The originator of the packet</param>
        private static async Task InvokeHandlerAsync(Handler handler, object packet, Player player)
        {
            var task = handler.Info.ReturnType == typeof(Task);
            var parameters = new object[] {packet, player};
            var res = handler.Info.Invoke(handler.Group, parameters);

            if (task)
            {
                if (res != null)
                {
                    await (Task) res;
                }
                else
                {
                    Logger.Error($"Handler returned null: {handler.PacketType.FullName}");
                }
            }
        }
    }
}