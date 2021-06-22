using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct FireEventClientSideMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.FireEventClientSide;
		[Wide]
		public string Arguments { get; set; }
		public GameObject Target { get; set; }
		[Default]
		public long Parameter1 { get; set; }
		[Default(-1)]
		public int Parameter2 { get; set; }
		public GameObject Sender { get; set; }
	}
}