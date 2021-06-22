using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ZonePropertyModelEquippedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ZonePropertyModelEquipped;
		[Default]
		public GameObject PlayerId { get; set; }
		[Default]
		public GameObject PropertyId { get; set; }
	}
}