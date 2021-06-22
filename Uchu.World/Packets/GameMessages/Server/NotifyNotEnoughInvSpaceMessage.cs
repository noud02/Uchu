using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyNotEnoughInvSpaceMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyNotEnoughInvSpace;
		public uint FreeSlotsNeeded { get; set; }
		[Default]
		public InventoryType InventoryType { get; set; }
	}
}