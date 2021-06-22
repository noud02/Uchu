using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct EnterProperty1Message
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.EnterProperty1;
		public int Index { get; set; }
		public bool ReturnToZone { get; set; }
	}
}