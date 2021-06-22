using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SellToVendorMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SellToVendor;
		[Default(1)]
		public int Count { get; set; }
		public Item Item { get; set; }
	}
}