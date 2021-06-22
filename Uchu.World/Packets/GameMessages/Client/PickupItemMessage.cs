namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PickupItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PickupItem;
		public GameObject Loot { get; set; }
		public Player Player { get; set; }
	}
}