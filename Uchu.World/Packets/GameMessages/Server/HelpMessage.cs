namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct HelpMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.Help;
		public int HelpId { get; set; }
	}
}