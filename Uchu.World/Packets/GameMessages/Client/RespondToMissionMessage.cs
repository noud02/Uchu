using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RespondToMissionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RespondToMission;
		public int MissionId { get; set; }
		public GameObject PlayerId { get; set; }
		public GameObject Receiver { get; set; }
		[Default]
		public Lot RewardItem { get; set; }
	}
}