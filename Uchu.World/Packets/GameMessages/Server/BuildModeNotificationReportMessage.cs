namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct BuildModeNotificationReportMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BuildModeNotificationReport;
		public bool Start { get; set; }
		public int NumSent { get; set; }
	}
}