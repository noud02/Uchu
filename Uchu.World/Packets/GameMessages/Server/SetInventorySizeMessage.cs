using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetInventorySizeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetInventorySize;
		public InventoryType InventoryType { get; set; }
		public int Size { get; set; }
	}
}