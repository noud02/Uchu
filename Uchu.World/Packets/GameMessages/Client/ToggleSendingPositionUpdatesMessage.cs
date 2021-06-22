using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ToggleSendingPositionUpdatesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ToggleSendingPositionUpdates;
		public bool SendUpdates { get; set; }
	}
}