using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Api.Models;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.Physics;
using Uchu.World.Client;
using Uchu.World.Filters;
using Uchu.World.Social;

namespace Uchu.World
{
    public sealed class Player : GameObject
    {
        public static async Task<Player> Instantiate(IRakConnection connection, Zone zone, ObjectId id)
        {
            // Set up a cancellation token for the connection disconnecting.
            // This may happen before the object starts.
            var cancellationToken = new CancellationTokenSource();
            connection.Disconnected += (disconnectionReason) =>
            {
                cancellationToken.Cancel();
                return Task.CompletedTask;
            };
            
            // Create base game object
            var instance = Instantiate<Player>(
                zone,
                position: zone.SpawnPosition,
                rotation: zone.SpawnRotation,
                scale: 1,
                objectId: id,
                lot: 1
            );

            await instance.LoadAsync(connection, cancellationToken.Token);
            return instance;
        }
        
        internal Player()
        {
            OnRespondToMission = new Event<int, GameObject, Lot>();
            OnFireServerEvent = new Event<string, FireEventServerSideMessage>();
            OnReadyForUpdatesEvent = new Event<ReadyForUpdatesMessage>();
            OnPositionUpdate = new Event<Vector3, Quaternion>();
            OnPetTamingTryBuild = new Event<PetTamingTryBuildMessage>();
            OnNotifyTamingBuildSuccessMessage = new Event<NotifyTamingBuildSuccessMessage>();
            OnLootPickup = new Event<Lot>();
            OnWorldLoad = new Event();

            Listen(OnStart, () =>
            {
                // Destroy the player on disconnect.
                // Also check if the player disconnected during loading.
                if (_loadCancellationToken != default && _loadCancellationToken.IsCancellationRequested)
                {
                    DestroyAsync().Wait();
                    return;
                }
                Connection.Disconnected += async reason =>
                {
                    await DestroyAsync();
                };

                Listen(OnPositionUpdate, UpdatePhysics);

                if (TryGetComponent<DestructibleComponent>(out var destructibleComponent))
                {
                    destructibleComponent.OnResurrect.AddListener(() => { GetComponent<DestroyableComponent>().Imagination = 6; });
                }
                
                // Save the player every 15 seconds
                Zone.Update(this, () =>
                {
                    // Done in the background as this takes long
                    Task.Run(async () =>
                    {
                        Logger.Debug($"Saving {this}.");
                        var timer = new Stopwatch();
                        timer.Start();
                        
                        await GetComponent<SaveComponent>().SaveAsync();

                        timer.Stop();
                        Logger.Debug($"Saved {this} in {timer.Elapsed:m\\:ss\\.fff}.");
                    });
                    
                    return Task.CompletedTask;
                }, 20 * 15);
                
                // Update the player view filters every five seconds
                Zone.Update(this, async () =>
                {
                    await Perspective.TickAsync();
                }, 20 * 5);
                
                // Check banned status every minute
                Zone.Update(this, async () =>
                {
                    if (Banned)
                    {
                        try
                        {
                            await Connection.CloseAsync();
                        }
                        catch (Exception e)
                        {
                            Logger.Error(e);
                        }
                    }
                }, 20);
            });
            
            Listen(OnDestroyed, () =>
            {
                OnFireServerEvent.Clear();
                OnLootPickup.Clear();
                OnWorldLoad.Clear();
                OnPositionUpdate.Clear();
            });
        }

        /// <summary>
        /// Destroys the player.
        /// </summary>
        public async Task DestroyAsync(CloseReason? reason = CloseReason.ClientDisconnect)
        {
            Logger.Information($"{this} left: {reason}.");
            await GetComponent<SaveComponent>().SaveAsync(false);
            Connection = default;
            Destroy(this);
        }

        /// <summary>
        /// Constructs the player, settings spawn parameters and masks
        /// </summary>
        /// <param name="character">The character that should be spawned</param>
        /// <param name="connection">User endpoint for this character</param>
        /// <param name="zone">The zone to spawn in</param>
        /// <returns>The constructed player</returns>
        public async Task LoadAsync(IRakConnection connection, CancellationToken cancellationToken)
        {
            await using var uchuContext = new UchuContext();
            var character = await uchuContext.Characters
                .SingleAsync(c => c.Id == Id);

            Connection = connection;
            Name = character.Name;
            _loadCancellationToken = cancellationToken;

            // Setup layers
            Layer = StandardLayer.Player;
            
            var layer = StandardLayer.All;
            layer -= StandardLayer.Hidden;
            layer -= StandardLayer.Spawner;
            
            Perspective = new Perspective(this);

            var maskFilter = Perspective.AddFilter<MaskFilter>();
            maskFilter.ViewMask = layer;

            Perspective.AddFilter<RenderDistanceFilter>();
            Perspective.AddFilter<ExcludeFilter>();
            
            // Add as first component so that it can be destructed first
            var saveComponent = AddComponent<SaveComponent>();

            // Add serialized components
            var controllablePhysics = AddComponent<ControllablePhysicsComponent>();
            AddComponent<DestructibleComponent>();
            var stats = GetComponent<DestroyableComponent>();
            AddComponent<CharacterComponent>();
            var inventory = AddComponent<InventoryComponent>();
            
            AddComponent<LuaScriptComponent>();
            AddComponent<SkillComponent>();
            AddComponent<RendererComponent>();
            AddComponent<PossessableOccupantComponent>();
            
            controllablePhysics.HasPosition = true;
            
            // Server Components
            var inventoryManager = AddComponent<InventoryManagerComponent>();
            await inventory.EquipItemsAsync(inventoryManager.Items.Where(i => i.IsEquipped));
            
            AddComponent<MissionInventoryComponent>();
            AddComponent<TeamPlayerComponent>();
            AddComponent<ModularBuilderComponent>();
            
            // Physics
            var physics = AddComponent<PhysicsComponent>();
            var box = CapsuleBody.Create(
                Zone.Simulation,
                Transform.Position,
                Transform.Rotation,
                new Vector2(2, 4)
            );

            physics.SetPhysics(box);
            
            Listen(physics.OnEnter, OnEnterCollision);
            Listen(physics.OnCollision, OnStayCollision);
            Listen(physics.OnLeave, OnLeaveCollision);

            // Register player game object in zone
            Start(this);
            Construct(this);
            
            // Register player as an active in zone
            await Zone.RegisterPlayer(this);
            
            // Once everything is done, allow saving this player
            saveComponent.StartSaving();
        }
        
        #region properties

        public Event<string, FireEventServerSideMessage> OnFireServerEvent { get; }
        
        public Event<ReadyForUpdatesMessage> OnReadyForUpdatesEvent { get; }

        public Event<int, GameObject, Lot> OnRespondToMission { get; }
        
        public Event<PetTamingTryBuildMessage> OnPetTamingTryBuild { get; }
        public Event<NotifyTamingBuildSuccessMessage> OnNotifyTamingBuildSuccessMessage { get; }

        public Event<Lot> OnLootPickup { get; }
        
        public Event OnWorldLoad { get; }

        public Event<Vector3, Quaternion> OnPositionUpdate { get; }

        public IRakConnection Connection { get; private set; }

        public Perspective Perspective { get; private set; }
        
        public PlayerChatChannel ChatChannel { get; set; }

        public GuildGuiState GuildGuiState { get; set; }
        
        public string GuildInviteName { get; set; }
        
        public int Ping => Connection.AveragePing;
        
        public override string Name
        {
            get => ObjectName;
            set => ObjectName = value;
        }

        /// <summary>
        /// Whether this player is banned or not
        /// </summary>
        public bool Banned { get; private set; }

        #endregion properties

        /// <summary>
        /// Internal gravity scale of the player
        /// </summary>
        private float _gravityScale = 1;

        /// <summary>
        /// Cancellation token for loading the player, such
        /// as if the player disconnects.
        /// </summary>
        private CancellationToken _loadCancellationToken;
        
        /// <summary>
        /// Gravity scale of the player
        /// </summary>
        public float GravityScale
        {
            get => _gravityScale;
            set
            {
                _gravityScale = Math.Clamp(value, 0, 2);
                
                Message(new SetGravityScaleMessage
                {
                    Associate = this,
                    Scale = _gravityScale
                });
            }
        }

        /// <summary>
        /// Triggers a celebration for the player
        /// </summary>
        /// <param name="celebrationId">The Id of the celebration to trigger</param>
        public async Task TriggerCelebration(CelebrationId celebrationId)
        {
            var celebrations = await ClientCache.GetTableAsync<CelebrationParameters>();
            var celebration = (celebrations.Where(t => t.Id == (int)celebrationId).ToArray())[0];

            this.Message(new StartCelebrationEffectMessage
            {
                Associate = this,
                Animation = celebration.Animation,
                BackgroundObject = new Lot(celebration.BackgroundObject.Value),
                CameraPathLot = new Lot(celebration.CameraPathLOT.Value),
                CeleLeadIn = celebration.CeleLeadIn.Value,
                CeleLeadOut = celebration.CeleLeadOut.Value,
                CelebrationId = celebration.Id.Value,
                Duration = celebration.Duration.Value,
                IconId = (uint) celebration.IconID.Value,
                MainText = celebration.MainText,
                MixerProgram = celebration.MixerProgram,
                MusicCue = celebration.MusicCue,
                PathNodeName = celebration.PathNodeName,
                SoundGuid = celebration.SoundGUID,
                SubText = celebration.SubText
            }); // Start effect
        }

        /// <summary>
        /// Called when the player remained inside a physics body
        /// </summary>
        /// <param name="other">The physics body that the player remained in</param>
        private void OnStayCollision(PhysicsComponent other)
        {
        }
        
        /// <summary>
        /// Called when a player enters a physics body
        /// </summary>
        /// <param name="other"></param>
        private void OnEnterCollision(PhysicsComponent other)
        {
        }
        
        /// <summary>
        /// Called when a player leaves a physics body
        /// </summary>
        /// <param name="other">The physics body that was left</param>
        private void OnLeaveCollision(PhysicsComponent other)
        {
        }

        /// <summary>
        /// Updates the physics of a player to the given position and rotation
        /// </summary>
        /// <param name="position">The position to set</param>
        /// <param name="rotation">The rotation to set</param>
        private void UpdatePhysics(Vector3 position, Quaternion rotation)
        {
            if (!(TryGetComponent<PhysicsComponent>(out var physicsComponent) && 
                  physicsComponent.Physics is PhysicsBody physics))
                return;

            physics.Position = Transform.Position;
            physics.Rotation = Transform.Rotation;

            var details = GetComponent<ControllablePhysicsComponent>();
            
            physics.AngularVelocity = details.HasAngularVelocity ? details.AngularVelocity : Vector3.Zero;
            physics.LinearVelocity = details.HasVelocity ? details.Velocity : Vector3.Zero;
        }

        /// <summary>
        /// Teleports the player to a different position
        /// </summary>
        /// <param name="position">The position to teleport the player to</param>
        public void Teleport(Vector3 position, bool ignore = false)
        {
            Message(new TeleportMessage
            {
                Associate = this,
                Position = position,
                IgnoreY = ignore
            });
        }

        /// <summary>
        /// Updates the view of the player, constructing objects that should be visible but aren't in the view and
        /// destructing objects that shouldn't be visible but are in the view
        /// </summary>
        internal void UpdateView()
        {
            var loadedObjects = Perspective.LoadedObjects.ToArray();
            foreach (var gameObject in Zone.Spawned)
            {
                TriggerViewUpdate(gameObject);
            }
        }
        
        /// <summary>
        /// Adds, removes or leaves a game object untouched based on whether it's in the view of a player
        /// </summary>
        /// <param name="gameObject">The game object to check</param>
        public void TriggerViewUpdate(GameObject gameObject)
        {
            var spawned = Perspective.LoadedObjects.ToArray().Contains(gameObject);
            var view = Perspective.View(gameObject);

            if (!spawned && view)
            {
                Zone.SendConstruction(gameObject, this);
            }
        }

        /// <summary>
        /// Sends a chat message
        /// </summary>
        /// <param name="message">The message to send</param>
        /// <param name="channel">The channel over which the player wished to communicate</param>
        /// <param name="author">The author, <c>null</c> for self</param>
        /// <param name="chatChannel">The world channel the player wishes to communicate over</param>
        public void SendChatMessage(string message, PlayerChatChannel channel = PlayerChatChannel.Debug, 
            Player author = null, ChatChannel chatChannel = World.ChatChannel.Public)
        {
            if (channel > ChatChannel)
                return;
            
            Message(new ChatMessagePacket
            {
                Message = $"{message}\0",
                Sender = author,
                IsMythran = author?.GameMasterLevel > 0,
                Channel = chatChannel
            });
        }

        /// <summary>
        /// Sends a packet to a player
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void Message(ISerializable packet)
        {
            if (Id == ObjectId.Invalid)
                return;
            Connection.Send(packet);
        }
        
        /// <summary>
        /// Sends a packet to a player
        /// </summary>
        /// <param name="packet">The packet to send</param>
        public void Message<T>(T packet) where T : struct
        {
            if (Id == ObjectId.Invalid)
                return;
            Connection.Send(packet);
        }

        /// <summary>
        /// Tries to send a player to a different zone
        /// </summary>
        /// <param name="zoneId"></param>
        /// <returns></returns>
        public async Task<bool> SendToWorldAsync(ZoneId zoneId)
        {
            Logger.Debug($"Requesting server for: {zoneId}");

            InstanceInfo server;
            try
            {
                server = await ServerHelper.RequestWorldServerAsync(UchuServer, zoneId);
                if (server == default)
                {
                    Logger.Debug($"Could not find server for: {zoneId}");
                    return false;
                }
            }
            catch (Exception e)
            {
                Logger.Error(e);
                return false;
            }
            
            Logger.Debug($"Yielded {server?.Port.ToString() ?? "<void>"} for {zoneId}");
            await SendToWorldAsync(server, zoneId);
            return true;
        }
        
        /// <summary>
        /// Sends a player to a different world, updating the zone id 
        /// </summary>
        /// <param name="serverInformation">Information regarding the server to connect to</param>
        /// <param name="zoneId">The zone id to travel to</param>
        public async Task SendToWorldAsync(InstanceInfo serverInformation, ZoneId zoneId)
        {
            // Don't redirect the user to a world they're already in
            if (UchuServer.Port == serverInformation.Port)
                return;
            
            GetComponent<CharacterComponent>().LastZone = zoneId;
            await GetComponent<SaveComponent>().SaveAsync(false);
            
            Message(new ServerRedirectionPacket
            {
                Port = (ushort) serverInformation.Port,
                Address = UchuServer.Host
            });
        }

        /// <summary>
        /// Updates the player name, used for custom names
        /// </summary>
        /// <param name="name">The player name to set</param>
        public void SetName(string name)
        {
            Name = name;
            Message(new SetNameMessage
            {
                Associate = this,
                Name = Name
            });
        }

        ~Player()
        {
            Logger.Information($"Player freed: {Id}");
        }
    }
}