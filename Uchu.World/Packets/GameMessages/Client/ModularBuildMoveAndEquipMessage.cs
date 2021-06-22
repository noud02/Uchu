using RakDotNet.IO;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct ModularBuildMoveAndEquipMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ModularBuildMoveAndEquip;
        public Lot Lot;
    }
}