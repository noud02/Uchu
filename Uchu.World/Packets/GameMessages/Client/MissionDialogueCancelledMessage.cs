using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct MissionDialogueCancelledMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MissionDialogueCancelled;
		public bool IsComplete { get; set; }
		public MissionState MissionState { get; set; }
		public int MissionId { get; set; }
		public GameObject Responder { get; set; }
	}
}