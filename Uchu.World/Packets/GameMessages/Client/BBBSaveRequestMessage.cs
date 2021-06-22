namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BbbSaveRequestMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BBBSaveRequest;
		public GameObject LocalId { get; set; }
		public byte[] LxfmlDataCompressed { get; set; }
		public uint TimeTakenInMs { get; set; }
	}
}