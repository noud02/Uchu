using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public struct PhantomPhysicsSerialization
    {
        public bool HasPosition { get; set; }
        [Requires("HasPosition")]
        public Vector3 Position { get; set; }
        [Requires("HasPosition")]
        public Quaternion Rotation { get; set; }
        public bool UnknownFlag { get; set; }
        [Requires("UnknownFlag")]
        public bool IsEffectActive { get; set; }
        [Requires("IsEffectActive")]
        public PhantomPhysicsEffectType EffectType { get; set; }
        [Requires("IsEffectActive")]
        public float EffectAmount { get; set; }
        [Requires("IsEffectActive")]
        public bool AffectedByDistance { get; set; }
        [Requires("AffectedByDistance")]
        public float MinDistance { get; set; }
        [Requires("AffectedByDistance")]
        public float MaxDistance { get; set; }
        [Default]
        [Requires("IsEffectActive")]
        public Vector3 EffectDirectionScaled { get; set; }
    }
}