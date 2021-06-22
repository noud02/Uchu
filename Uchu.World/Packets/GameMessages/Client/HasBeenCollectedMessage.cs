namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct HasBeenCollectedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.HasBeenCollected;
		public GameObject PlayerId { get; set; }
	}
}