namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BbbLoadItemRequestMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BBBLoadItemRequest;
		public GameObject ItemId { get; set; }
	}
}