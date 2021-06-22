namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct NotifyTamingModelLoadedOnServerMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.NotifyTamingModelLoadedOnServer;
    }
}