using RakDotNet.IO;

namespace Uchu.World
{
    public class CancelRailMovementMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.CancelRailMovement;

        public bool Immediate { get; set; }

        public override void Deserialize(BitReader reader)
        {
            Immediate = reader.ReadBit();
        }
    }
}