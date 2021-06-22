namespace Uchu.World
{
    [ServerGameMessagePacketStruct]
    public struct DoneLoadingObjectsMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ServerDoneLoadingAllObjects;
    }
}