using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class DetailedUserInfoPacket : Packet
    {
        public override RemoteConnectionType RemoteConnectionType => RemoteConnectionType.Server;

        public override uint PacketId => 0x4;

        public LegoDataDictionary Data { get; set; }

        public override void SerializePacket(BitWriter writer)
        {
            writer.WriteLdfCompressed(Data);
        }
    }
}
