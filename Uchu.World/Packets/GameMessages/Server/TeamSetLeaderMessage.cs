namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeamSetLeaderMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TeamSetLeader;
		public GameObject NewLeader { get; set; }
	}
}