using System;
using System.Linq;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.World.Client;

namespace Uchu.World
{
    public class RailActivatorComponent : ReplicaComponent
    {
        public override ComponentId Id => ComponentId.RailActivator;

        // from object settings
        private int RailActivatorComponentId;

        private bool ActivatorActive;

        private bool ActivatorDamageImmune;

        private string LoopSound;

        private bool NoAggro;

        private bool NotifyActivatorArrived;

        private string Path;

        private bool PathDirection;

        private uint PathStart;

        private bool ShowNameBillboard;

        private bool UseDb;

        // from cdclient
        private int? StartEffectId;
        private string StartEffectType;

        private int? DuringEffectId;
        private string DuringEffectType;

        private int? StopEffectId;
        private string StopEffectType;

        private string StartSound;
        private string StopSound;

        private string StartAnim;
        private string LoopAnim;
        private string StopAnim;

        private bool CameraLocked;
        private bool CollisionEnabled;

        protected RailActivatorComponent()
        {
            Listen(OnStart, async () =>
            {
                RailActivatorComponentId = GameObject.Lot.GetComponentId(ComponentId.RailActivator);

                var railComponent = (await ClientCache.GetTableAsync<Core.Client.RailActivatorComponent>())
                    .First(c => c.Id == RailActivatorComponentId);

                ActivatorDamageImmune = (bool) GameObject.Settings["rail_activator_damage_immune"];
                LoopSound = (string) GameObject.Settings["rail_loop_sound"];
                NoAggro = (bool) GameObject.Settings["rail_no_aggro"];
                NotifyActivatorArrived = (bool) GameObject.Settings["rail_notify_activator_arrived"];
                Path = (string) GameObject.Settings["rail_path"];
                // TODO: investigate why PathDirection 0 makes the movement a straight line instead of the reverse path
                PathDirection = (bool) GameObject.Settings["rail_path_direction"];
                PathStart = (uint) GameObject.Settings["rail_path_start"];
                ShowNameBillboard = (bool) GameObject.Settings["rail_show_name_billboard"];
                UseDb = (bool) GameObject.Settings["rail_use_db"];

                StartAnim = railComponent.StartAnim;
                LoopAnim = railComponent.LoopAnim;
                StopAnim = railComponent.StopAnim;

                StartSound = railComponent.StartSound;
                StopSound = railComponent.StopSound;

                // these 3 fields can be null
                var startEffect = railComponent.StartEffectID;
                if (startEffect != null)
                {
                    var split = startEffect.Split(':');
                    StartEffectId = int.Parse(split[0]);
                    StartEffectType = split[1];
                }

                var duringEffect = railComponent.EffectIDs;
                if (duringEffect != null)
                {
                    var split = duringEffect.Split(':');
                    DuringEffectId = int.Parse(split[0]);
                    DuringEffectType = split[1];
                }

                var stopEffect = railComponent.StopEffectID;
                if (stopEffect != null)
                {
                    var split = stopEffect.Split(':');
                    StopEffectId = int.Parse(split[0]);
                    StopEffectType = split[1];
                }

                CameraLocked = railComponent.CameraLocked ?? true;
                CollisionEnabled = railComponent.PlayerCollision ?? false;

                Listen(GameObject.OnInteract, OnInteract);
            });
        }

        private void OnInteract(Player player)
        {
            Logger.Information($"Path: {Path}, Start: {PathStart}, Direction: {PathDirection}");
            if (GameObject.TryGetComponent<QuickBuildComponent>(out var quickBuildComponent)
                && quickBuildComponent.State != RebuildState.Completed)
                return;

            // Start: "Begin" effect
            if (StartEffectId != null)
                player.Message(new PlayFXEffectMessage
                {
                    Associate = player,
                    EffectId = (int) StartEffectId,
                    EffectType = StartEffectType,
                    Name = StartEffectId.ToString(),
                });

            // Start: "Begin" animation
            player.Message(new PlayAnimationMessage
            {
                Associate = player,
                AnimationsId = StartAnim,
                ExpectAnimationToExist = true,
                PlayImmediate = false,
                TriggerOnCompleteMessage = false,
            });

            player.Message(new StartRailMovementMessage
            {
                Associate = player,

                DamageImmune = ActivatorDamageImmune,
                NoAggro = NoAggro,
                NotifyActivator = NotifyActivatorArrived,
                ShowNameBillboard = ShowNameBillboard,
                CameraLocked = CameraLocked,
                CollisionEnabled = CollisionEnabled,
                PathGoForward = PathDirection,
                UseDb = UseDb,

                StartSound = StartSound,
                StopSound = StopSound,
                LoopSound = LoopSound,

                PathName = Path,
                RailActivator = GameObject,
                RailActivatorComponent = RailActivatorComponentId,

            });

            // Wait for ClientRailMovementReady
            Delegate clientReady = null;
            clientReady = Listen(player.OnRailMovementReady, () =>
            {
                ReleaseListener(clientReady);
                // Start: "Loop" animation
                player.Message(new PlayAnimationMessage
                {
                    Associate = player,
                    AnimationsId = LoopAnim,
                    ExpectAnimationToExist = true,
                    PlayImmediate = false,
                    TriggerOnCompleteMessage = false,
                });

                // Start: "During" effect
                if (DuringEffectId != null)
                    player.Message(new PlayFXEffectMessage
                    {
                        Associate = player,
                        EffectId = (int) DuringEffectId,
                        EffectType = DuringEffectType,
                        Name = DuringEffectId.ToString(),
                    });

                // Start: Actual movement
                player.Message(new SetRailMovementMessage
                {
                    Associate = player,
                    PathGoForward = PathDirection,
                    PathName = Path,
                    PathStart = PathStart,
                    RailActivator = GameObject,
                    RailActivatorComponent = RailActivatorComponentId,
                });
            });

            // Wait for CancelRailMovement
            Delegate clientFinished = null;
            clientFinished = Listen(player.OnCancelRailMovement, () =>
            {
                ReleaseListener(clientFinished);
                // Stop: "During" effect
                player.Message(new StopFXEffectMessage
                {
                    Associate = player,
                    Name = DuringEffectId.ToString(),
                });

                // Start: "End" effect
                if (StopEffectId != null)
                    player.Message(new PlayFXEffectMessage
                    {
                        Associate = player,
                        EffectId = (int) StopEffectId,
                        EffectType = StopEffectType,
                        Name = StopEffectId.ToString(),
                    });

                // Start: "End" animation
                player.Message(new PlayAnimationMessage
                {
                    Associate = player,
                    AnimationsId = StopAnim,
                    ExpectAnimationToExist = true,
                    PlayImmediate = false,
                    TriggerOnCompleteMessage = false,
                });
            });
        }

        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}