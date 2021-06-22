namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ZoneSummaryDismissedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ZoneSummaryDismissed;
		public GameObject PlayerId { get; set; }
	}
}