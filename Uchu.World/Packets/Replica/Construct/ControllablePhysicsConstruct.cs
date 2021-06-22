using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public struct ControllablePhysicsConstruct
    {
        public bool HasJetpackEffect { get; set; }
        [Requires("HasJetpackEffect")]
        public uint JetpackEffectId { get; set; }
        [Requires("HasJetpackEffect")]
        public bool Flying { get; set; }
        [Requires("HasJetpackEffect")]
        public bool BypassFlyingChecks { get; set; }
        public bool UnknownFlag1 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint1 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint2 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint3 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint4 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint5 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint6 { get; set; }
        [Requires("UnknownFlag1")]
        public uint UnknownUint7 { get; set; }
        public bool HasSpeedOrGravityMultiplier { get; set; }
        [Requires("HasSpeedOrGravityMultiplier")]
        public float GravityMultiplier { get; set; }
        [Requires("HasSpeedOrGravityMultiplier")]
        public float SpeedMultiplier { get; set; }
        public bool UnknownFlag2 { get; set; }
        [Requires("UnknownFlag2")]
        public uint UnknownUint8 { get; set; }
        [Requires("UnknownFlag2")]
        public bool UnknownFlag3 { get; set; }
        public bool UnknownFlag4 { get; set; }
        [Requires("UnknownFlag4")]
        public bool UnknownFlag5 { get; set; }
        [Requires("UnknownFlag5")]
        public uint UnknownUint9 { get; set; }
        [Requires("UnknownFlag5")]
        public bool UnknownFlag6 { get; set; }
        
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
        public bool UnknownFlag7 { get; set; }
    }
}