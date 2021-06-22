using System.Numerics;
using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    [PacketStruct(MessageIdentifier.UserPacketEnum, RemoteConnectionType.Client, 0x16)]
    public struct PositionUpdatePacket
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public bool IsOnGround { get; set; }
        public bool NegativeAngularVelocity { get; set; }
        [Default]
        public Vector3 Velocity { get; set; }
        [Default]
        public Vector3 AngularVelocity { get; set; }
        public bool IsOnPlatform { get; set; }
        [Requires("IsOnPlatform")]
        public long PlatformObjectId { get; set; }
        [Requires("IsOnPlatform")]
        public Vector3 PlatformPosition { get; set; }
        [Requires("IsOnPlatform")]
        [Default]
        public Vector3 UnknownVector3 { get; set; }
    }
}