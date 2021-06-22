using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestActivitySummaryLeaderboardDataMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestActivitySummaryLeaderboardData;
		[Default]
		public int GameId { get; set; }
		[Default(QueryType.TopCharacter)]
		public QueryType QueryType { get; set; }
		[Default(10)]
		public int ResultsEnd { get; set; }
		[Default]
		public int ResultsStart { get; set; }
		public GameObject Target { get; set; }
		public bool Weekly { get; set; }
	}
}