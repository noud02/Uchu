using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BuyFromVendorMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BuyFromVendor;
		public bool Confirmed { get; set; }
		[Default(1)]
		public int Count { get; set; }
		public Lot Lot { get; set; }
	}
}