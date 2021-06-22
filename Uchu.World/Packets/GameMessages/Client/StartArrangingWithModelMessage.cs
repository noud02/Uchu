using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct StartArrangingWithModelMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.StartArrangingWithModel;
        public Item Item { get; set; }
    }
}