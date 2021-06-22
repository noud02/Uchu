using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct VerifyAckMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VerifyAck;
		public bool Different { get; set; }
		public byte[] Bitstream { get; set; }
		[Default]
		public uint Handle { get; set; }
	}
}