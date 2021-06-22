namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeamPickupItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TeamPickupItem;
		public GameObject LootId { get; set; }
		public GameObject LootOwnerId { get; set; }
	}
}