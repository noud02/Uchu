using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class RemoveItemToInventoryMessage : GeneralGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.RemoveItemFromInventory;

        public bool Confirmed { get; set; }

        public bool DeleteItem { get; set; } = true;

        public bool OutSuccess { get; set; }

        public InventoryType InventoryType { get; set; } = InventoryType.Items;

        public ItemType ItemType { get; set; } = ItemType.Invalid;

        public LegoDataDictionary ExtraInfo { get; set; }

        public bool ForceDeletion { get; set; } = true;

        public GameObject Source { get; set; }

        public Item Item { get; set; }

        public Lot ItemLot { get; set; } = -1;

        public GameObject Requesting { get; set; }

        public uint Delta { get; set; } = 1;

        public uint TotalItems { get; set; }

        public long SubKey { get; set; } = -1;

        public long TradeId { get; set; } = -1;

        public override void SerializeMessage(BitWriter writer)
        {
            writer.WriteBit(Confirmed);
            writer.WriteBit(DeleteItem);
            writer.WriteBit(OutSuccess);

            writer.WriteBit(true);
            writer.Write((int) InventoryType);

            writer.WriteBit(true);
            writer.Write((int) ItemType);

            if (ExtraInfo != null)
            {
                Uchu.Core.BitWriterExtensions.Write(writer, ExtraInfo);

                writer.Write<byte>(0);
                writer.Write<byte>(0);
            }
            else
            {
                writer.Write<uint>(0);
            }

            writer.WriteBit(ForceDeletion);

            var hasSource = Source != default;
            writer.WriteBit(hasSource);
            if (hasSource) writer.Write(Source);

            writer.WriteBit(true);
            writer.Write(Item);

            var hasLot = ItemLot != -1;
            writer.WriteBit(hasLot);
            if (hasLot) writer.Write(ItemLot);

            var hasRequesting = Requesting != default;
            writer.WriteBit(hasRequesting);
            if (hasRequesting) writer.Write(Requesting);

            writer.WriteBit(true);
            writer.Write(Delta);

            writer.WriteBit(true);
            writer.Write(TotalItems);

            var hasSubKey = SubKey != -1;
            writer.WriteBit(hasSubKey);
            if (hasSubKey) writer.Write(SubKey);

            var hasTradeId = TradeId != -1;
            writer.WriteBit(hasTradeId);
            if (hasTradeId) writer.Write(TradeId);
        }

        public override void Deserialize(BitReader reader)
        {
            Confirmed = reader.ReadBit();
            DeleteItem = reader.ReadBit();
            OutSuccess = reader.ReadBit();

            if (reader.ReadBit()) InventoryType = (InventoryType) reader.Read<int>();

            if (reader.ReadBit()) ItemType = (ItemType) reader.Read<int>();

            var len = reader.Read<uint>();
            if (len > 0)
            {
                var info = reader.ReadString((int) len, true);
                ExtraInfo = LegoDataDictionary.FromString(info);
            }

            ForceDeletion = reader.ReadBit();

            if (reader.ReadBit()) Source = reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit()) Item = reader.ReadGameObject<Item>(Associate.Zone);

            if (reader.ReadBit()) Requesting = reader.ReadGameObject(Associate.Zone);

            if (reader.ReadBit()) TotalItems = reader.Read<uint>();

            if (reader.ReadBit()) SubKey = reader.Read<long>();

            if (reader.ReadBit()) TradeId = reader.Read<long>();
        }
    }
}
