namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetEmoteLockStateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetEmoteLockState;
		public bool Lock { get; set; }
		public int EmoteId { get; set; }
	}
}