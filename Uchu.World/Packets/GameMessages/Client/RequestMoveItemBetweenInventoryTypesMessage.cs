using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestMoveItemBetweenInventoryTypesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestMoveItemBetweenInventoryTypes;
		public bool AllowPartial { get; set; }
		[Default(-1)]
		public int DestinationSlot { get; set; }
		[Default(1)]
		public uint Count { get; set; }
		[Default]
		public InventoryType Destination { get; set; }
		[Default]
		public InventoryType Source { get; set; }
		[Default]
		public Item Item { get; set; }
		public bool ShowFlyingLoot { get; set; }
		[Default]
		public ObjectId Subkey { get; set; }
		[Default]
		public Lot TemplateId { get; set; }
	}
}