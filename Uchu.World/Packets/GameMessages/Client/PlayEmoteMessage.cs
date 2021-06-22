namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PlayEmoteMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayEmote;
		public int EmoteId { get; set; }
		public GameObject Target { get; set; }
	}
}