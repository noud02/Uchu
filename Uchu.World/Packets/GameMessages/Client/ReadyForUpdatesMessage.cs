namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ReadyForUpdatesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ReadyForUpdates;
		public GameObject GameObject { get; set; }
	}
}