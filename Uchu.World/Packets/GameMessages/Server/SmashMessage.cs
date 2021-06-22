using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SmashMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.Smash;
		public bool IgnoreObjectVisibility { get; set; }
		public float Force { get; set; }
		public float GhostOpacity { get; set; }
		public GameObject KillerId { get; set; }
	}
}