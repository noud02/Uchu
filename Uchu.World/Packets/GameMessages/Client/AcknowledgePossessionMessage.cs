using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct AcknowledgePossessionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.AcknowledgePossession;
		[Default]
		public GameObject PossessedObjId { get; set; }
	}
}