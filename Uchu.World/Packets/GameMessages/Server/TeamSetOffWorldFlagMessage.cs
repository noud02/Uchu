using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeamSetOffWorldFlagMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TeamSetOffWorldFlag;
		public GameObject PlayerId { get; set; }
		public ZoneId ZoneId { get; set; }
	}
}