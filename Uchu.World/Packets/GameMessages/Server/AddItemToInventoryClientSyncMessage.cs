using InfectedRose.Lvl;
using Uchu.Core;
using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct AddItemToInventoryClientSyncMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.AddItemToInventoryClientSync;
		public bool IsBound { get; set; }
		public bool IsBoundOnEquip { get; set; }
		public bool IsBoundOnPickup { get; set; }
		[Default]
		public LootType Source { get; set; }
		public LegoDataDictionary ExtraInfo { get; set; }
		public Lot ItemLot { get; set; }
		[Default]
		public GameObject Subkey { get; set; }
		[Default]
		public InventoryType InventoryType { get; set; }
		[Default(1)]
		public uint Delta { get; set; }
		[Default]
		public uint TotalItems { get; set; }
		public Item Item { get; set; }
		public Vector3 FlyingLootPosit { get; set; }
		public bool ShowFlyingLoot { get; set; }
		public int Slot { get; set; }
	}
}