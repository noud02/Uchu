namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetUserCtrlCompPauseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetUserCtrlCompPause;
		public bool Paused { get; set; }
	}
}