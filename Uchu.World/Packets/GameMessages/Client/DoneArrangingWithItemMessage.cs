using Uchu.Core;
using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct DoneArrangingWithItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DoneArrangingWithItem;
		public InventoryType NewSourceBag { get; set; }
		public GameObject NewSourceId { get; set; }
		public Lot NewSourceLot { get; set; }
		public int NewSourceType { get; set; }
		public GameObject NewTargetId { get; set; }
		public Lot NewTargetLot { get; set; }
		public int NewTargetType { get; set; }
		public Vector3 NewTargetPos { get; set; }
		public InventoryType OldItemBag { get; set; }
		public GameObject OldItemId { get; set; }
		public Lot OldItemLot { get; set; }
		public int OldItemType { get; set; }
	}
}