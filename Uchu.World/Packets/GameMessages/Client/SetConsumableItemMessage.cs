namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetConsumableItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetConsumableItem;
		public Lot Lot { get; set; }
	}
}