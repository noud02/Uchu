namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyClientFlagChangeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyClientFlagChange;
		public bool Flag { get; set; }
		public int FlagId { get; set; }
	}
}