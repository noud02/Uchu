using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetCurrencyMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetCurrency;
		public long Currency { get; set; }
		[Default]
		public LootType LootType { get; set; }
		public Vector3 Position { get; set; }
		[Default]
		public Lot SourceLot { get; set; }
		[Default]
		public GameObject SourceObject { get; set; }
		[Default]
		public GameObject SourceTradeId { get; set; }
		[Default]
		public LootType SourceType { get; set; }
	}
}