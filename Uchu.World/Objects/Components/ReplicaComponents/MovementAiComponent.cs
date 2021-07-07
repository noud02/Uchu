using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class MovementAiComponent : ReplicaComponent
    {
        private const int UpdateRate = 2;
        
        public override ComponentId Id => ComponentId.MovementAIComponent;
        
        private MovementAIComponent ClientInfo { get; set; }
        
        private BaseCombatAiComponent BaseCombatAiComponent { get; set; }
        
        private ControllablePhysicsComponent ControllablePhysicsComponent { get; set; }
        
        private DestructibleComponent DestructibleComponent { get; set; }

        private Random Random { get; }

        public float TetherRadius { get; set; } = 65;

        public float WanderRadius { get; set; } = 60;

        public float AggroRadius { get; set; } = 55;

        public float ConductRadius { get; set; } = 15;

        public float TetherSpeed { get; set; } = 8;

        public float AggroDistance { get; set; } = 7;

        public float AggroSpeed { get; set; } = 3.5f;

        public float WanderChange { get; set; } = 100;

        public float WanderDelayMin { get; set; } = 5;

        public float WanderDelayMax { get; set; } = 5;

        public float WanderSpeed { get; set; } = 0.5f;
        
        public bool WanderDelay { get; private set; }

        public float Speed
        {
            get
            {
                return BaseCombatAiComponent.Action switch
                {
                    CombatAiAction.Idle => WanderSpeed * 2,
                    CombatAiAction.Attacking => (TetherSpeed * AggroSpeed),
                    CombatAiAction.Tether => TetherSpeed,
                    CombatAiAction.Spawn => 0,
                    CombatAiAction.Dead => 0,
                    _ => throw new ArgumentOutOfRangeException(nameof(BaseCombatAiComponent.Action))
                };
            }
        }

        public Vector3[] Path { get; private set; } = new Vector3[0];
        
        public int PathIndex { get; private set; }

        public Vector3 CurrentWayPoint
        {
            get
            {
                if (Path.Length == default || PathIndex >= Path.Length) return Transform.Position;
                
                return Path[PathIndex];
            }
        }

        public bool HasWayPoint => !(Path.Length == default || PathIndex >= Path.Length);

        public Vector3 Origin { get; private set; }
        
        private Event OnEndOfPath { get; }
        
        private Event OnEndOfWayPoint { get; }
        
        private Event Regular { get; }
        
        private float DeltaTime { get; set; }
        
        protected MovementAiComponent()
        {
            Random = new Random();
            
            OnEndOfPath = new Event();
            
            OnEndOfWayPoint = new Event();
            
            Regular = new Event();
            
            Listen(OnStart, () =>
            {
                Listen(GameObject.OnStart, async () =>
                {
                    if (Zone.NavMeshManager == default || !Zone.NavMeshManager.Enabled) return;

                    var componentId = GameObject.Lot.GetComponentId(ComponentId.MovementAIComponent);
                    var info = await ClientCache.FindAsync<MovementAIComponent>(componentId);

                    if (info == default)
                    {
                        Destroy(this);

                        return;
                    }

                    BaseCombatAiComponent = GameObject.GetComponent<BaseCombatAiComponent>();

                    ControllablePhysicsComponent = GameObject.GetComponent<ControllablePhysicsComponent>();

                    DestructibleComponent = GameObject.GetComponent<DestructibleComponent>();

                    ClientInfo = info;

                    Origin = Transform.Position;

                    var destructible = GameObject.GetComponent<DestructibleComponent>();

                    ControllablePhysicsComponent.HasVelocity = true;

                    Listen(destructible.OnSmashed, (smasher, lootOwner) =>
                    {
                        if (!GameObject.Alive) return;

                        Transform.Position = Origin;

                        BaseCombatAiComponent.Target = null;

                        ControllablePhysicsComponent.Velocity = Vector3.Zero;

                        ControllablePhysicsComponent.HasVelocity = true;
                    });

                    SetOnTick(CalculateAction);

                    Zone.Update(GameObject, delta =>
                    {
                        Regular.Invoke();

                        CalculateMovement(delta);

                        return Task.CompletedTask;
                    }, UpdateRate);
                });
            });
        }

        private void CalculateMovement(float deltaTime)
        {
            if (!Zone.NavMeshManager.Enabled) return;
            
            if (CannotPerformAction)
            {
                if (ControllablePhysicsComponent.Velocity == Vector3.Zero) return;
                
                ControllablePhysicsComponent.Velocity = Vector3.Zero;

                ControllablePhysicsComponent.HasPosition = true;

                GameObject.Serialize(GameObject);

                return;
            }
            
            if (Vector3.Distance(Transform.Position, CurrentWayPoint) < 1)
            {
                PathIndex++;
                
                OnEndOfWayPoint.Invoke();
                
                if (PathIndex >= Path.Length)
                {
                    OnEndOfPath.Invoke();
                }

                if (HasWayPoint)
                {
                    Transform.LookAt(CurrentWayPoint);
                }
            }

            DeltaTime += deltaTime;

            var newPosition = Transform.Position.MoveTowards(CurrentWayPoint, Speed * DeltaTime, out var deltaVector);

            if (Vector3.Distance(Transform.Position, newPosition) < 0.25f)
            {
                return;
            }

            DeltaTime = 0;

            var delta = deltaVector * Speed;
            
            ControllablePhysicsComponent.HasVelocity = delta != Vector3.Zero;
            
            ControllablePhysicsComponent.Velocity = delta;

            ControllablePhysicsComponent.HasPosition = newPosition != Transform.Position;

            Transform.Position = newPosition;

            GameObject.Serialize(GameObject);
        }

        private void CalculateAction()
        {
            if (CannotPerformAction)
            {
                SetOnTick(CalculateAction);
            }
            
            var target = SearchForTarget();

            if (!(target is Player))
            {
                if (WanderDelay || !UchuServer.Config.GamePlay.AiWander) return;
                
                Wander();
                
                return;
            }

            Transform.LookAt(target.Transform.Position);

            if (CanTether(target))
            {
                if (CanAttack(target))
                {
                    Attack(target);

                    return;
                }

                Tether(target);

                return;
            }

            if (Vector3.Distance(Transform.Position, Origin) > 10)
                Retreat();
        }

        private void Wander()
        {
            /*
             * This sometimes creates unreachable paths, making the path finding stall.
             */
            
            if (WanderDelay) return;
            
            var value = Random.Next(0, 100);

            WanderDelay = true;
            
            if (WanderChange < value)
            {
                var _ = Task.Run(CalculateWanderDelay);
                
                return;
            }
            
            var r = Math.Sqrt((double) Random.Next() / int.MaxValue) * WanderRadius;
            var t = (double) Random.Next() / int.MaxValue * 2 * Math.PI;
            var position = new Vector3((float) (r * Math.Cos(t)), 0, (float) (r * Math.Sin(t)));

            position += Origin;

            BaseCombatAiComponent.Target = default;
            
            BaseCombatAiComponent.Action = CombatAiAction.Idle;

            GameObject.Serialize(GameObject);

            CalculatePath(position);

            var __ = Task.Run(CalculateWanderDelay);

            SetOnTick(CalculateAction);
        }

        private async Task CalculateWanderDelay()
        {
            var delay = Random.Next((int) (WanderDelayMin * 1000), (int) (WanderDelayMax * 1000) + 1);

            await Task.Delay(delay);

            WanderDelay = false;
        }

        private GameObject SearchForTarget()
        {
            var validTargets = BaseCombatAiComponent.SeekValidTargets();
            
            var targets = validTargets.Where(target =>
            {
                var transform = target.Transform;

                var distance = Vector3.Distance(transform.Position, Transform.Position);

                return distance <= AggroRadius;
            }).ToList();

            targets.ToList().Sort((g1, g2) =>
            {
                var distance1 = Vector3.Distance(g1.Transform.Position, Transform.Position);
                var distance2 = Vector3.Distance(g2.Transform.Position, Transform.Position);

                return (int) (distance2 - distance1);
            });

            return targets.FirstOrDefault();
        }

        private bool CanTether(GameObject gameObject)
        {
            return Vector3.Distance(gameObject.Transform.Position, Origin) <= TetherRadius;
        }

        private bool CanAttack(GameObject gameObject)
        {
            return Vector3.Distance(gameObject.Transform.Position, Transform.Position) <= ConductRadius;
        }

        private void Attack(GameObject gameObject)
        {
            BaseCombatAiComponent.Action = CombatAiAction.Attacking;

            BaseCombatAiComponent.Target = gameObject;
            
            GameObject.Serialize(gameObject);

            CalculatePath(gameObject.Transform.Position);

            SetOnEndOfPath(CalculateAction);
        }

        private void Tether(GameObject gameObject)
        {
            BaseCombatAiComponent.Action = CombatAiAction.Tether;
            
            BaseCombatAiComponent.Target = gameObject;

            GameObject.Serialize(gameObject);

            CalculatePath(gameObject.Transform.Position);
            
            SetOnEndOfWayPoint(CalculateAction);
        }

        private void Retreat()
        {
            BaseCombatAiComponent.Action = CombatAiAction.Attacking;

            BaseCombatAiComponent.Target = default;

            GameObject.Serialize(GameObject);

            CalculatePath(Origin);

            SetOnEndOfPath(CalculateAction);
        }

        private void CalculatePath(Vector3 target)
        {
            var watch = new Stopwatch();

            watch.Start();

            var closest = Zone.NavMeshManager.FindClosestNode(Transform.Position);

            if (Vector3.Distance(closest, Transform.Position) > 10)
            {
                return;
            }

            Logger.Debug($"Calculating path to: {target}");
            
            Path = Zone.NavMeshManager.GeneratePath(Transform.Position, target);

            Logger.Debug($"Finished calculated path to: {target} in {watch.ElapsedMilliseconds}ms!");

            PathIndex = 1;
        }

        private void SetOnEndOfPath(Action action)
        {
            OnEndOfPath.Clear();
            OnEndOfWayPoint.Clear();
            Regular.Clear();

            Listen(OnEndOfPath, action);
        }

        private void SetOnEndOfWayPoint(Action action)
        {
            OnEndOfPath.Clear();
            OnEndOfWayPoint.Clear();
            Regular.Clear();

            Listen(OnEndOfWayPoint, action);
        }

        private void SetOnTick(Action action)
        {
            OnEndOfPath.Clear();
            OnEndOfWayPoint.Clear();
            Regular.Clear();

            Listen(Regular, action);
        }

        private bool CannotPerformAction => !DestructibleComponent.Alive || BaseCombatAiComponent.AbilityDowntime;
        
        public override void Construct(BitWriter writer)
        {
        }

        public override void Serialize(BitWriter writer)
        {
        }
    }
}