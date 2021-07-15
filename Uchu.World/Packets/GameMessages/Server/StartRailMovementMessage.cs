using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class StartRailMovementMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.StartRailMovement;

        public bool DamageImmune { get; set; } = true;

        public bool NoAggro { get; set; } = true;

        public bool NotifyActivator { get; set; } = false;

        public bool ShowNameBillboard { get; set; } = true;

        public bool CameraLocked { get; set; } = true;

        public bool CollisionEnabled { get; set; } = true;

        public string LoopSound { get; set; }

        public bool PathGoForward { get; set; } = true;

        public string PathName { get; set; }

        public uint? PathStart { get; set; } // default 0

        public int? RailActivatorComponent { get; set; } // default -1

        public GameObject RailActivator { get; set;} // default obj id empty

        public string StartSound { get; set; }

        public string StopSound { get; set; }

        public bool UseDb { get; set; } = true;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(DamageImmune);

            writer.WriteBit(NoAggro);

            writer.WriteBit(NotifyActivator);

            writer.WriteBit(ShowNameBillboard);

            writer.WriteBit(CameraLocked);

            writer.WriteBit(CollisionEnabled);

            writer.Write((uint) LoopSound.Length);
            writer.WriteString(LoopSound, LoopSound.Length, true);

            writer.WriteBit(PathGoForward);

            writer.Write((uint) PathName.Length);
            writer.WriteString(PathName, PathName.Length, true);

            writer.WriteBit(PathStart != null);
            if (PathStart != null)
                writer.Write((uint) PathStart);

            writer.WriteBit(RailActivatorComponent != null);
            if (RailActivatorComponent != null)
                writer.Write((int) RailActivatorComponent);

            writer.WriteBit(RailActivator != null);
            if (RailActivator != null)
                writer.Write(RailActivator);

            writer.Write((uint) StartSound.Length);
            writer.WriteString(StartSound, StartSound.Length, true);

            writer.Write((uint) StopSound.Length);
            writer.WriteString(StopSound, StopSound.Length, true);

            writer.WriteBit(UseDb);
        }
    }
}