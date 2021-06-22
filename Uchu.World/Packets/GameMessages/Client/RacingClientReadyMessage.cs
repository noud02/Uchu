namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RacingClientReadyMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RacingClientReady;
		public GameObject PlayerId { get; set; }
	}
}