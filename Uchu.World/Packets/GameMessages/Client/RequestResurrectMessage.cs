namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct RequestResurrectMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.RequestResurrect;
    }
}