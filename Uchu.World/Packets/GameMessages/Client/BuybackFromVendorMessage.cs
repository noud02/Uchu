using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct BuybackFromVendorMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.BuybackFromVendor;
        public bool Confirmed { get; set; }
        [Default(1)]
        public int Count { get; set; }
        public Item Item { get; set; }
    }
}