using Uchu.Core;
using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct FinishArrangingWithItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.FinishArrangingWithItem;
		[Default]
		public GameObject BuildArea { get; set; }
		public InventoryType NewSourceBag { get; set; }
		public GameObject NewSource { get; set; }
		public Lot NewSourceLot { get; set; }
		public int NewSourceType { get; set; }
		public GameObject NewTarget { get; set; }
		public Lot NewTargetLot { get; set; }
		public int NewTargetType { get; set; }
		public Vector3 NewTargetPosition { get; set; }
		public InventoryType OldItemBag { get; set; }
		public GameObject OldItemId { get; set; }
		public Lot OldItemLot { get; set; }
		public int OldItemType { get; set; }
	}
}