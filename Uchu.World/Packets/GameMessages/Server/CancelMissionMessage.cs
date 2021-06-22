namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct CancelMissionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.CancelMission;
		public int MissionId { get; set; }
		public bool ResetCompleted { get; set; }
	}
}