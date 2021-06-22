namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct UseNonEquipmentItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UseNonEquipmentItem;
		public Item Item { get; set; }
	}
}