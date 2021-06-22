using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct MoveItemInInventoryMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.MoveItemInInventory;
        [Default(InventoryType.Invalid)]
        public InventoryType DestinationInventoryType { get; set; }
        public Item Item { get; set; }
        public InventoryType CurrentInventoryType { get; set; }
        public int ResponseCode { get; set; }
        public int NewSlot { get; set; }
    }
}