namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct RestoreToPostLoadStatsMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RestoreToPostLoadStats;
    }
}