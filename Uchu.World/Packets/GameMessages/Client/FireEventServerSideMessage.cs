using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct FireEventServerSideMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.FireEventServerSide;
		[Wide]
		public string Arguments { get; set; }
		[Default(-1)]
		public int Param1 { get; set; }
		[Default(-1)]
		public int Param2 { get; set; }
		[Default(-1)]
		public int Param3 { get; set; }
		public GameObject SenderId { get; set; }
	}
}