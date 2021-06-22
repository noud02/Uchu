namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TerminateInteractionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TerminateInteraction;
		public GameObject Terminator { get; set; }
		public TerminateType Type { get; set; }
	}
}