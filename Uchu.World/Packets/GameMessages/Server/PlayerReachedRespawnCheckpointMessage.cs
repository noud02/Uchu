using System.Numerics;
using RakDotNet.IO;

namespace Uchu.World
{
    public class PlayerReachedRespawnCheckpointMessage: ServerGameMessage

    {
        public override GameMessageId GameMessageId => GameMessageId.PlayerReachedRespawnCheckpoint;

        public Vector3 Position { get; set; }

        public Quaternion Rotation { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            writer.Write(Position);
            writer.Write(Rotation);
        }
    }
}
