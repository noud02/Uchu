namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct OpenVendorWindowMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.VendorOpenWindow;
    }
}