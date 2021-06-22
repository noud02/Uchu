namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PlayerLoadedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayerLoaded;
		public GameObject Player { get; set; }
	}
}