namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ClientItemConsumedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ClientItemConsumed;
		public Item Item { get; set; }
	}
}