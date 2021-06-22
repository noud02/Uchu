namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RacingResetPlayerToLastResetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RacingResetPlayerToLastReset;
		public GameObject PlayerId { get; set; }
	}
}