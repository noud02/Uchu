using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ToggleInteractionUpdatesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ToggleInteractionUpdates;
		public bool Enable { get; set; }
	}
}