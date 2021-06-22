using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeamAddPlayerMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TeamAddPlayer;
		public bool IsFreeTrial { get; set; }
		public bool Local { get; set; }
		public bool NoLootOnDeath { get; set; }
		public Player Player { get; set; }
		[Wide]
		public string PlayerName { get; set; }
		[Default]
		public ZoneId ZoneId { get; set; }
	}
}