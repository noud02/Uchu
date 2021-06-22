namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct PetTamingTryBuildMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.PetTamingTryBuild;
        public Brick[] Bricks { get; set; }
        public bool Failed;
    }
}