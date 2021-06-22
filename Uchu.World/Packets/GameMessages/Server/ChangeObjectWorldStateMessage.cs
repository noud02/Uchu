using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ChangeObjectWorldStateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ChangeObjectWorldState;
		[Default]
		public ObjectWorldState State { get; set; }
	}
}