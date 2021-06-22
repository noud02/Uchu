using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ResetPropertyBehaviorsMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ResetPropertyBehaviors;
		public bool Force { get; set; }
		public bool Pause { get; set; }
	}
}