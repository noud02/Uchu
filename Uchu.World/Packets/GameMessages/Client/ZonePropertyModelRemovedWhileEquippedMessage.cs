using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ZonePropertyModelRemovedWhileEquippedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ZonePropertyModelRemovedWhileEquipped;
		[Default]
		public GameObject PlayerId { get; set; }
		[Default]
		public GameObject PropertyId { get; set; }
	}
}