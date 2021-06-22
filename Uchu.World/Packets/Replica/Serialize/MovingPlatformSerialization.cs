using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public struct MovingPlatformSerialization
    {
        public bool UnknownFlag1 { get; set; }
        public bool HasPath { get; set; }
        [Wide]
        // [Default]
        [StoreLengthAs(typeof(ushort))]
        [Requires("HasPath")]
        public string PathName { get; set; }
        [Requires("HasPath")]
        public uint PathStart { get; set; }
        [Requires("HasPath")]
        public bool UnknownFlag2 { get; set; }
        [Default]
        public PlatformType Type { get; set; }
        
        public bool UnknownFlag3 { get; set; }
        [Requires("Type", PlatformType.SimpleMover)]
        public bool UnknownFlag4 { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public PlatformState State { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public int UnknownInt { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public bool UnknownFlag5 { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public bool NotAtBeginning { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public float UnknownFloat { get; set; }
        public Vector3 TargetPosition { get; set; }
        [Requires("Type", PlatformType.SimpleMover)]
        public Quaternion TargetRotation { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public uint CurrentWaypointIndex { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public uint NextWaypointIndex { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public float IdleTimeElapsed { get; set; }
        [Requires("Type", PlatformType.Mover)]
        public uint UnknownUint { get; set; }
        [Requires("Type", PlatformType.SimpleMover)]
        public bool UnknownFlag6 { get; set; }
    }
}