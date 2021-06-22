using InfectedRose.Lvl;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct MatchUpdateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MatchUpdate;
		public LegoDataDictionary Data { get; set; }
		public MatchUpdateType Type { get; set; }
	}
}