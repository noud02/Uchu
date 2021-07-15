using System.Numerics;
using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class AddItemToInventoryMessage : ServerGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.AddItemToInventoryClientSync;

        public bool IsBound { get; set; }

        public bool IsBoundOnEquip { get; set; }

        public bool IsBoundOnPickup { get; set; }

        public LootType Source { get; set; } = LootType.None;

        public LegoDataDictionary ExtraInfo { get; set; } = null;

        public int ItemLot { get; set; }

        public long SubKey { get; set; } = -1;

        public int InventoryType { get; set; } = -1;

        public uint Delta { get; set; } = 1;

        public uint TotalItems { get; set; }

        public Item Item { get; set; }

        public Vector3 FlyingLootPosition { get; set; } = Vector3.Zero;

        public bool ShowFlyingLoot { get; set; } = true;

        public int Slot { get; set; }

        public override void SerializeMessage(BitWriter writer)
        {
            /*
             * I don't know why this is not working... you can add new stacks, but cannot add to existent once.
             */

            writer.WriteBit(IsBound);
            writer.WriteBit(IsBoundOnEquip);
            writer.WriteBit(IsBoundOnPickup);

            writer.WriteBit(false);

            if (ExtraInfo != null)
            {
                var ldf = ExtraInfo.ToString();

                writer.Write((uint) ldf.Length);

                if (ldf.Length > 0)
                {
                    writer.WriteString(ldf, ldf.Length, true);

                    writer.Write((byte) 0);
                    writer.Write((byte) 0);
                }
            }
            else
            {
                writer.Write(0u);
            }

            writer.Write(ItemLot);

            writer.WriteBit(false);

            writer.WriteBit(true);
            writer.Write(InventoryType);

            writer.WriteBit(true);
            writer.Write(Delta);

            writer.WriteBit(true);
            writer.Write(TotalItems);

            writer.Write(Item.Id);

            writer.Write(FlyingLootPosition);

            writer.WriteBit(ShowFlyingLoot);

            writer.Write(Slot);
        }
    }
}
