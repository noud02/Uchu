namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetTooltipFlagMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetTooltipFlag;
		public bool Flag { get; set; }
		public int ToolTip { get; set; }
	}
}