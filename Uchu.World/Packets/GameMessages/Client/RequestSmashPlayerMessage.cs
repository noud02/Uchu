namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct RequestSmashPlayerMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RequestSmashPlayer;
    }
}