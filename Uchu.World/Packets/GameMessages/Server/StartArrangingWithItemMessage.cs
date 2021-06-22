using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct StartArrangingWithItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StartArrangingWithItem;
		public bool FirstTime { get; set; }
		[Default]
		public GameObject BuildArea { get; set; }
		public Vector3 StartPosition { get; set; }
		public InventoryType SourceBag { get; set; }
		public GameObject Source { get; set; }
		public Lot SourceLot { get; set; }
		public int SourceType { get; set; }
		public GameObject Target { get; set; }
		public Lot TargetLot { get; set; }
		public Vector3 TargetPosition { get; set; }
		public int TargetType { get; set; }
	}
}