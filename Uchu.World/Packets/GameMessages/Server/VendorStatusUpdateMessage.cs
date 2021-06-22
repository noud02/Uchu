namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct VendorStatusUpdateMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.VendorStatusUpdate;
        public bool UpdateOnly { get; set; }
        public ShopEntry[] Entries { get; set; }
    }
}