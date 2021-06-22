using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyRailActivatorStateChangeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyRailActivatorStateChange;
		public bool Active { get; set; }
	}
}