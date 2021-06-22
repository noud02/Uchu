using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public struct ControllablePhysicsSerialization
    {
        public bool HasSpeedOrGravityMultiplier { get; set; }
        [Requires("HasSpeedOrGravityMultiplier")]
        public float GravityMultiplier { get; set; }
        [Requires("HasSpeedOrGravityMultiplier")]
        public float SpeedMultiplier { get; set; }
        public bool UnknownFlag1 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint1 { get; set; }
        [Requires("UnknownFlag1")]
        public bool UnknownFlag2 { get; set; }
        public bool UnknownFlag3 { get; set; }
        [Requires("UnknownFlag3")]
        public bool UnknownFlag4 { get; set; }
        [Requires("UnknownFlag4")]
        public uint UnknownUint2 { get; set; }
        [Requires("UnknownFlag4")]
        public bool UnknownFlag5 { get; set; }
        public bool HasPosition { get; set; }
        [Requires("HasPosition")]
        public Vector3 Position { get; set; }
        [Requires("HasPosition")]
        public Quaternion Rotation { get; set; }
        [Requires("HasPosition")]
        public bool IsOnGround { get; set; }
        [Requires("HasPosition")]
        public bool IsOnRail { get; set; }
        [Requires("HasPosition")]
        [Default]
        public Vector3 Velocity { get; set; }
        [Requires("HasPosition")]
        [Default]
        public Vector3 AngularVelocity { get; set; }
        [Requires("HasPosition")]
        public bool HasPlatform { get; set; }
        [Requires("HasPlatform")]
        public GameObject Platform { get; set; }
        [Requires("HasPlatform")]
        public Vector3 PlatformPosition { get; set; }
        [Requires("HasPlatform")]
        public bool UnknownFlag6 { get; set; }
        public bool UnknownFlag7 { get; set; }
    }
}