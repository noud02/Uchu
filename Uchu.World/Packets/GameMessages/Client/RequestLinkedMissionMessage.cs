namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestLinkedMissionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestLinkedMission;
		public GameObject PlayerId { get; set; }
		public int MissionId { get; set; }
		public bool MissionOffered { get; set; }
	}
}