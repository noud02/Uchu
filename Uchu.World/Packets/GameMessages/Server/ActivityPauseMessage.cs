namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ActivityPauseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ActivityPause;
		public bool Pause { get; set; }
	}
}