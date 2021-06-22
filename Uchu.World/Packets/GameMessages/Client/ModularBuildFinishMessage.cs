using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct ModularBuildFinishMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.ModularBuildFinish;
        [StoreLengthAs(typeof(byte))]
        public Lot[] Modules { get; set; }
    }
}