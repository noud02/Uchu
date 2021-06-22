using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct DropClientLootMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DropClientLoot;
		public bool UsePosition { get; set; }
		[Default]
		public Vector3 FinalPosition { get; set; }
		public int Currency { get; set; }
		public Lot Lot { get; set; }
		public GameObject Loot { get; set; }
		public GameObject Owner { get; set; }
		public GameObject Source { get; set; }
		[Default]
		public Vector3 SpawnPosition { get; set; }
	}
}