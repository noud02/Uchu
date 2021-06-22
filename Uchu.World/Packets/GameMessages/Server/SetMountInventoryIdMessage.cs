using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetMountInventoryIdMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetMountInventoryId;
		[Default]
		public GameObject InventoryMountId { get; set; }
	}
}