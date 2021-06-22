using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RemoveItemFromInventoryMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RemoveItemFromInventory;
        public bool Confirmed { get; set; }
        public bool DeleteItem { get; set; }
        public bool OutSuccess { get; set; }
        [Default(InventoryType.Invalid)]
        public InventoryType InventoryType { get; set; }
        [Default]
        public ItemType ItemType { get; set; }
        public LegoDataDictionary ExtraInfo { get; set; }
        public bool ForceDeletion { get; set; }
        [Default]
        public GameObject Source { get; set; }
        [Default]
        public Item Item { get; set; }
        [Default]
        public Lot ItemLot { get; set; }
        [Default]
        public GameObject Requesting { get; set; }
        [Default(1)]
        public uint Delta { get; set; }
        [Default]
        public uint TotalItems { get; set; }
        [Default]
        public long SubKey { get; set; }
        [Default]
        public long TradeId { get; set; }
    }
}