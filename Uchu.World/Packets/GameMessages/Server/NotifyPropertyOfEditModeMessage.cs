namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyPropertyOfEditModeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyPropertyOfEditMode;
		public bool EditingActive { get; set; }
	}
}