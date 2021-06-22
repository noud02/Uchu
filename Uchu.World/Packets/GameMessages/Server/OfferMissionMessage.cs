namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct OfferMissionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.OfferMission;
		public int MissionId { get; set; }
		public GameObject QuestGiver { get; set; }
	}
}