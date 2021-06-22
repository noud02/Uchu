namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BuildExitConfirmationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BuildExitConfirmation;
		public GameObject PlayerId { get; set; }
	}
}