using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using InfectedRose.Luz;
using RakDotNet.IO;
using Uchu.Core;
using Timer = System.Timers.Timer;

namespace Uchu.World
{
    public class MovingPlatformComponent : ReplicaComponent
    {
        /// <summary>
        ///     Current timer.
        /// </summary>
        private Timer Timer { get; set; }

        /// <summary>
        ///     Current stopwatch for delta times.
        /// </summary>
        private Stopwatch Stopwatch { get; set; } = new Stopwatch();

        public LuzMovingPlatformPath Path { get; set; }

        public string PathName { get; set; }

        public uint PathStart { get; set; }

        public PlatformType Type { get; set; }

        public PlatformState State { get; set; } = PlatformState.Idle;

        public Vector3 TargetPosition { get; set; }

        public Quaternion TargetRotation { get; set; }

        public uint CurrentWaypointIndex { get; set; }

        public uint NextWaypointIndex { get; set; }

        /// <summary>
        ///     Time spent idle
        /// </summary>
        public float IdleTimeElapsed =>
            State == PlatformState.Idle
                ? (float) (Stopwatch.ElapsedMilliseconds / 1000.0)
                : 0;

        /// <summary>
        ///     Percent of the platform's moving progress between the current waypoints
        /// </summary>
        public float PercentBetweenPoints => State != PlatformState.Idle
            ? (float) ((Stopwatch.ElapsedMilliseconds / 1000.0) / _currentDuration)
            : 0;

        public override ComponentId Id => ComponentId.MovingPlatformComponent;

        /// <summary>
        ///     Next Path Index
        /// </summary>
        private uint NextIndex => CurrentWaypointIndex + 1 > Path.Waypoints.Length - 1 ? 0 : CurrentWaypointIndex + 1;

        /// <summary>
        ///     Current WayPoint
        /// </summary>
        public LuzMovingPlatformWaypoint WayPoint => Path.Waypoints[CurrentWaypointIndex] as LuzMovingPlatformWaypoint;

        /// <summary>
        ///     Next WayPoint
        /// </summary>
        public LuzMovingPlatformWaypoint NextWayPoint => Path.Waypoints[NextIndex] as LuzMovingPlatformWaypoint;

        private double _currentDuration;

        protected MovingPlatformComponent()
        {
            Listen(OnStart, () =>
            {
                PathName = GameObject.Settings.TryGetValue("attached_path", out var name) ? (string) name : "";
                PathStart = GameObject.Settings.TryGetValue("attached_path_start", out var start)
                    ? Convert.ToUInt32(start)
                    : 0;

                Path = Zone.ZoneInfo.LuzFile.PathData.FirstOrDefault(p =>
                    p is LuzMovingPlatformPath && p.PathName == PathName) as LuzMovingPlatformPath;

                Type = !GameObject.Settings.TryGetValue("platformIsMover", out var isMover) || (bool) isMover
                    ? PlatformType.Mover
                    : GameObject.Settings.TryGetValue("platformIsSimpleMover", out var isSimpleMover) &&
                      (bool) isSimpleMover
                        ? PlatformType.SimpleMover
                        : PlatformType.None;

                CurrentWaypointIndex = PathStart;

                State = PlatformState.Idle;
                
                Task.Run(() => WaitPoint());

                if (GameObject.TryGetComponent<SimplePhysicsComponent>(out var simplePhysicsComponent))
                {
                    simplePhysicsComponent.HasPosition = false;
                }

                if (!GameObject.TryGetComponent<QuickBuildComponent>(out var quickBuildComponent)) return;
                Listen(quickBuildComponent.OnStateChange, (state) =>
                {
                    if (state != RebuildState.Completed) return;
                    
                    // The waypoint must be set to the previous from the start since WaitPoint increments it.
                    this.Stop();
                    CurrentWaypointIndex = PathStart == 0 ? (uint) (Path.Waypoints.Length - 1) : PathStart - 1;
                    Task.Run(() =>
                    {
                        // TODO: A wait is required at the beginning, otherwise the platform moves right as the player is unfrozen from the building complete animation.
                        WaitPoint(1);
                    });
                });
            });
        }

        public override void Construct(BitWriter writer) 
        {
            Serialize(writer, true);
        }

        public override void Serialize(BitWriter writer)
        {
            Serialize(writer, false);
        }
        
        public void Serialize(BitWriter writer,bool includePath)
        {
            writer.WriteBit(true);

            var hasPath = includePath && PathName != null;

            writer.WriteBit(hasPath);

            if (hasPath)
            {
                writer.WriteBit(true);
                writer.Write((ushort) PathName.Length);
                writer.WriteString(PathName, PathName.Length, true);
                writer.Write(PathStart);
                writer.WriteBit(false);
            }

            var hasPlatform = Type != PlatformType.None;

            writer.WriteBit(hasPlatform);

            if (!hasPlatform) return;

            writer.Write((uint) Type);

            switch (Type)
            {
                case PlatformType.None:
                    break;

                case PlatformType.Mover:
                    writer.WriteBit(true);

                    writer.Write((uint) State);

                    writer.Write(-1);
                    writer.WriteBit(false);
                    writer.WriteBit(CurrentWaypointIndex != 0);
                    writer.Write<float>(0);

                    writer.Write(TargetPosition);

                    writer.Write(CurrentWaypointIndex);
                    writer.Write(NextWaypointIndex);

                    writer.Write(IdleTimeElapsed);
                    writer.Write<uint>(0);
                    break;

                case PlatformType.SimpleMover:
                    writer.WriteBit(true);
                    writer.WriteBit(true);

                    writer.Write(TargetPosition);
                    writer.Write(TargetRotation);

                    writer.WriteBit(false);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void Stop()
        {
            Timer.Stop();
            State = PlatformState.Idle;
        }

        public void MoveTo(uint position,Action moveCompleteCallback = default)
        {
            // Update Object in world.
            CurrentWaypointIndex = position;
            NextWaypointIndex = NextIndex;
            _currentDuration = (WayPoint.Position - NextWayPoint.Position).Length() / WayPoint.Speed;
            Stopwatch.Restart();
            State = PlatformState.Move;
            TargetPosition = WayPoint.Position;
            TargetRotation = WayPoint.Rotation;

            GameObject.Serialize(GameObject);

            // Start Waiting after completing path.
            Timer = new Timer
            {
                AutoReset = false,
                Interval = _currentDuration * 1000
            };
            Timer.Elapsed += (sender, args) =>
            {
                Timer.Stop();
                
                // Update Object in world.
                CurrentWaypointIndex = NextIndex;
                PathName = null;
                State = PlatformState.Idle;
                TargetPosition = WayPoint.Position;
                TargetRotation = WayPoint.Rotation;
                NextWaypointIndex = NextIndex;
            
                GameObject.Serialize(GameObject);
            };
            if (moveCompleteCallback != default)
            {
                Timer.Elapsed += (sender, args) => moveCompleteCallback();
            }

            Timer.Start();
        }

        private void MovePlatform()
        {
            // Update Object in world.
            _currentDuration = (WayPoint.Position - NextWayPoint.Position).Length() / WayPoint.Speed;
            Stopwatch.Restart();
            State = PlatformState.Move;
            TargetPosition = WayPoint.Position;
            TargetRotation = WayPoint.Rotation;
            NextWaypointIndex = NextIndex;

            GameObject.Serialize(GameObject);

            // Start Waiting after completing path.
            Timer = new Timer
            {
                AutoReset = false,
                Interval = _currentDuration * 1000
            };
            Timer.Elapsed += (sender, args) => { WaitPoint(); };

            Timer.Start();
        }

        private void WaitPoint(int extraWaitTime = 0)
        {
            // Move to next path index.
            CurrentWaypointIndex = NextIndex;
            Stopwatch.Restart();
            _currentDuration = WayPoint.Wait;
            
            // Update Object in world.
            State = PlatformState.Idle;
            TargetPosition = WayPoint.Position;
            TargetRotation = WayPoint.Rotation;
            NextWaypointIndex = NextIndex;
            
            GameObject.Serialize(GameObject);

            // Start Waiting after waiting.
            Timer = new Timer
            {
                AutoReset = false,
                Interval = (WayPoint.Wait * 1000) + extraWaitTime
            };

            Timer.Elapsed += (sender, args) => { MovePlatform(); };

            Timer.Start();
        }
    }
}
