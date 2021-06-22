namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ServerTerminateInteractionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ServerTerminateInteraction;
		public GameObject ObjIdTerminator { get; set; }
		public TerminateType TerminateType { get; set; }
	}
}