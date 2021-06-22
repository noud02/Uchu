namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct RequestPlatformResyncMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RequestPlatformResync;
    }
}