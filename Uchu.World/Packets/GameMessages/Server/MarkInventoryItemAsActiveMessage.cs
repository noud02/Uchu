using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct MarkInventoryItemAsActiveMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MarkInventoryItemAsActive;
		public bool Active { get; set; }
		[Default]
		public UnequippableActiveType UnequippableActiveType { get; set; }
		[Default]
		public GameObject ItemId { get; set; }
	}
}