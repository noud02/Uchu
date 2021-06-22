namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct StartServerPetMinigameTimerMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.StartServerPetMinigameTimer;
    }
}