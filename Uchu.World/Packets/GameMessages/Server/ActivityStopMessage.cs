namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ActivityStopMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ActivityStop;
		public bool Exit { get; set; }
		public bool UserCancel { get; set; }
	}
}