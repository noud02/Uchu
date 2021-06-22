namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct MatchResponseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MatchResponse;
		public MatchResponseType Response { get; set; }
	}
}