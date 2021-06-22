using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ZonePropertyModelRotatedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ZonePropertyModelRotated;
		[Default]
		public GameObject PlayerId { get; set; }
		[Default]
		public GameObject PropertyId { get; set; }
	}
}