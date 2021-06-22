using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ModifyPlayerZoneStatisticMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ModifyPlayerZoneStatistic;
		public bool Set { get; set; }
		[Wide]
		public string StatName { get; set; }
		[Default]
		public int StatValue { get; set; }
		[Default]
		public ZoneId ZoneId { get; set; }
	}
}