using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ChangeIdleFlagsMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ChangeIdleFlags;
		[Default]
		public int Off { get; set; }
		[Default]
		public int On { get; set; }
	}
}