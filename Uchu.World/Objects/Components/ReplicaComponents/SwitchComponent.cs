using System.Numerics;
using System.Threading.Tasks;
using RakDotNet.IO;
using Uchu.Physics;

namespace Uchu.World
{
    public class SwitchComponent : StructReplicaComponent<SwitchSerialize>
    {
        public bool State { get; set; }

        public bool IsHitSwitch { get; set; }
        
        public bool IsPressureSwitch { get; set; }
        
        public bool IsTimedSwitch { get; set; }
        
        public int SwitchResetTime { get; set; }

        public uint SwitchMaxUsers { get; set; } = 20;

        public uint SwitchUserRequirement { get; set; } = 1;
        
        public Player Activator { get; set; }

        public Event<Player> OnActivated { get; }
        
        public Event OnDeactivated { get; }

        public override ComponentId Id => ComponentId.SwitchComponent;

        protected SwitchComponent()
        {
            OnActivated = new Event<Player>();

            OnDeactivated = new Event();

            Listen(OnStart, () =>
            {
                if (GameObject.Settings.TryGetValue("is_hit_switch", out var isHit))
                {
                    IsHitSwitch = (bool) isHit;
                }
                
                if (GameObject.Settings.TryGetValue("is_pressure_switch", out var isPressure))
                {
                    IsPressureSwitch = (bool) isPressure;
                }
                
                if (GameObject.Settings.TryGetValue("is_timed_switch", out var isTimed))
                {
                    IsTimedSwitch = (bool) isTimed;
                }
                
                if (GameObject.Settings.TryGetValue("switch_reset_time", out var resetTime))
                {
                    SwitchResetTime = (int) resetTime;
                }
                
                if (GameObject.Settings.TryGetValue("switch_users_max", out var maxUsers))
                {
                    SwitchMaxUsers = (uint) maxUsers;
                }
                
                if (GameObject.Settings.TryGetValue("switch_users_required", out var requiredUsers))
                {
                    SwitchUserRequirement = (uint) requiredUsers;
                }

                var physics = this.GameObject.AddComponent<PhysicsComponent>();

                var size = Vector3.One;

                var physicsObject = BoxBody.Create(
                    this.GameObject.Zone.Simulation,
                    this.GameObject.Transform.Position,
                    this.GameObject.Transform.Rotation,
                    size
                );

                physics.SetPhysics(physicsObject);

                Listen(physics.OnEnter, async other =>
                {
                    if (!(other.GameObject is Player player)) return;

                    this.GameObject.TryGetComponent<QuickBuildComponent>(out var buttonQuickBuild);
                    if (buttonQuickBuild != default && buttonQuickBuild.State != RebuildState.Completed) return;
                    
                    await Activate(player);
                });
            });
        }

        /// <summary>
        ///     Activate this switch
        /// </summary>
        /// <remarks>
        ///     Invokes Deactivate after SwitchResetTime
        /// </remarks>
        /// <param name="player">The player that activates this switch</param>
        /// <returns></returns>
        public async Task Activate(Player player)
        {
            if (State) return;

            Activator = player;
            
            State = true;

            GameObject.Serialize(GameObject);

            await OnActivated.InvokeAsync(player);

            var _ = Task.Run(async () =>
            {
                await Task.Delay(SwitchResetTime * 1000);
                
                await Deactivate();
            });
        }

        /// <summary>
        ///     Deactivate this switch
        /// </summary>
        /// <returns></returns>
        public async Task Deactivate()
        {
            if (!State) return;

            State = false;

            GameObject.Serialize(GameObject);
            
            await OnDeactivated.InvokeAsync();
        }
    }
}