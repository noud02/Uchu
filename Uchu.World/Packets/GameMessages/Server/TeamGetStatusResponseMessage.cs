using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeamGetStatusResponseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TeamGetStatusResponse;
		public GameObject LeaderId { get; set; }
		public ZoneId LeaderZoneId { get; set; }
		public byte[] TeamBuffer { get; set; }
		public ushort LootFlag { get; set; }
		public ushort NumOfOtherPlayers { get; set; }
		[Wide]
		public string LeaderName { get; set; }
	}
}