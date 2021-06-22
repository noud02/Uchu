using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct MissionDialogueOkMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MissionDialogueOK;
		public bool IsComplete { get; set; }
		public MissionState MissionState { get; set; }
		public int MissionId { get; set; }
		public GameObject Responder { get; set; }
	}
}