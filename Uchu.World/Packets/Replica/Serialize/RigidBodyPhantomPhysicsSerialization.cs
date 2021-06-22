using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
    public struct RigidBodyPhantomPhysicsSerialization
    {
        public bool HasPosition { get; set; }
        [Requires("HasPosition")]
        public Vector3 Position { get; set; }
        [Requires("HasPosition")]
        public Quaternion Rotation { get; set; }
    }
}