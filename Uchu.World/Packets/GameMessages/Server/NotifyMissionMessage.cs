using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyMissionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyMission;
		public int MissionId { get; set; }
		public MissionState MissionState { get; set; }
		public bool SendingRewards { get; set; }
	}
}