namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct StartActivityTimeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StartActivityTime;
		public float StartTime { get; set; }
	}
}