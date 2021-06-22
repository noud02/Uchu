using InfectedRose.Lvl;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct MatchRequestMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.MatchRequest;
        public GameObject Activator { get; set; }
        public LegoDataDictionary Settings { get; set; }
        public MatchRequestType Type { get; set; }
        public int Value { get; set; }
    }
}