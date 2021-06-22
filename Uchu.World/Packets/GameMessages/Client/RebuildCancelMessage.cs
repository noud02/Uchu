namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RebuildCancelMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RebuildCancel;
		public bool EarlyRelease { get; set; }
		public GameObject UserId { get; set; }
	}
}