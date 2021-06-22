namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct EmotePlayedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.EmotePlayed;
		public int EmoteId { get; set; }
		public GameObject Target { get; set; }
	}
}