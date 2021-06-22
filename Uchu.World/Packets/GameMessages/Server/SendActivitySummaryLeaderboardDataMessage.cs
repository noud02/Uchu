using InfectedRose.Lvl;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SendActivitySummaryLeaderboardDataMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SendActivitySummaryLeaderboardData;
		public int GameId { get; set; }
		public int InfoType { get; set; }
		public LegoDataDictionary LeaderboardData { get; set; }
		public bool Throttled { get; set; }
		public bool Weekly { get; set; }
	}
}