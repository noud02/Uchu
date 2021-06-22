namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct BouncerActiveStatusMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BouncerActiveStatus;
		public bool Active { get; set; }
	}
}