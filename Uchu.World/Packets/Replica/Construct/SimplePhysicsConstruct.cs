using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public struct SimplePhysicsConstruct
    {
        public bool UnknownFlag1 { get; set; }
        public float UnknownFloat { get; set; }
        public bool HasVelocity { get; set; }
        [Requires("HasVelocity")]
        public Vector3 LinearVelocity { get; set; }
        [Requires("HasVelocity")]
        public Vector3 AngularVelocity { get; set; }
        [Default]
        public uint AirSpeed { get; set; }
        public bool HasPosition { get; set; }
        [Requires("HasPosition")]
        public Vector3 Position { get; set; }
        [Requires("HasPosition")]
        public Quaternion Rotation { get; set; }
    }
}