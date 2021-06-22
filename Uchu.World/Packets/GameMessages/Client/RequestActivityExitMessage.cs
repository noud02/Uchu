namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestActivityExitMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestActivityExit;
		public bool UserCancel { get; set; }
		public GameObject UserId { get; set; }
	}
}