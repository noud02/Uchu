namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetFlagMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetFlag;
		public bool Flag { get; set; }
		public int FlagId { get; set; }
	}
}