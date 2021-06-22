namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RacingPlayerInfoResetFinishedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RacingPlayerInfoResetFinished;
		public GameObject PlayerId { get; set; }
	}
}