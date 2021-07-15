using InfectedRose.Core;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public class MatchRequestMessage : ClientGameMessage
    {
        public override GameMessageId GameMessageId => GameMessageId.MatchRequest;

        public MatchRequestType Type { get; set; }
        
        public int Value { get; set; }
        
        public GameObject Activator { get; set; }
        
        public LegoDataDictionary Settings { get; set; }
        
        public override void Deserialize(BitReader reader)
        {
            Activator = reader.ReadGameObject(Associate.Zone);

            var ldl = reader.ReadString((int) reader.Read<uint>(), true);

            Settings = LegoDataDictionary.FromString(ldl);
            
            Type = (MatchRequestType) reader.Read<int>();

            Value = reader.Read<int>();
        }
    }
}
