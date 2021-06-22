namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct HasBeenCollectedByClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.HasBeenCollectedByClient;
		public GameObject PlayerId { get; set; }
	}
}