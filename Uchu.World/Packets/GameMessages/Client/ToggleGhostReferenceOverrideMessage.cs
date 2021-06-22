using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ToggleGhostReferenceOverrideMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ToggleGhostReferenceOverride;
		public bool Override { get; set; }
	}
}