namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RacingPlayerLoadedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RacingPlayerLoaded;
		public GameObject PlayerId { get; set; }
		public GameObject VehicleId { get; set; }
	}
}