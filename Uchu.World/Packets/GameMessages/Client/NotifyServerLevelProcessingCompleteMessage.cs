namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct NotifyServerLevelProcessingCompleteMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.NotifyServerLevelProcessingComplete;
    }
}
