namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UpdateReputationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UpdateReputation;
		public long Reputation { get; set; }
	}
}