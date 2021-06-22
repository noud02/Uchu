namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UIMessageServerToSingleClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UIMessageServerToSingleClient;
		public byte[] Content { get; set; }
		public string MessageName { get; set; }
	}
}