namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RebuildNotifyStateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RebuildNotifyState;
		public RebuildState CurrentState { get; set; }
		public RebuildState NewState { get; set; }
		public GameObject Player { get; set; }
	}
}