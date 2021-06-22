using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct DisplayZoneSummaryMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DisplayZoneSummary;
		public bool IsPropertyMap { get; set; }
		public bool IsZoneStart { get; set; }
		[Default]
		public GameObject Sender { get; set; }
	}
}