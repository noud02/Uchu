namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetBBBAutosaveMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetBBBAutosave;
		public byte[] LxfmlDataCompressed { get; set; }
	}
}