using Uchu.Core;
using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct StartBuildingWithItemMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StartBuildingWithItem;
		public bool FirstTime { get; set; }
		public bool Success { get; set; }
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