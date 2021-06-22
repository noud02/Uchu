using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UnSmashMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UnSmash;
		[Default]
		public GameObject BuilderId { get; set; }
		[Default(3)]
		public float Duration { get; set; }
	}
}