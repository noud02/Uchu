using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class SetRailMovementMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId { get; } = GameMessageId.SetRailMovement;

        public bool PathGoForward { get; set; } = true;

        public string PathName { get; set; }

        public uint PathStart { get; set; }

        public int? RailActivatorComponent { get; set; } // default -1

        public GameObject RailActivator { get; set;} // default 0

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(PathGoForward);

            writer.Write((uint) PathName.Length);
            writer.WriteString(PathName, PathName.Length, true);

            writer.Write(PathStart);

            writer.WriteBit(RailActivatorComponent != null);
            if (RailActivatorComponent != null)
                writer.Write((int) RailActivatorComponent);

            writer.WriteBit(RailActivator != null);
            if (RailActivator != null)
                writer.Write(RailActivator);
        }
    }
}