namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct PlayerReadyMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.PlayerReady;
    }
}