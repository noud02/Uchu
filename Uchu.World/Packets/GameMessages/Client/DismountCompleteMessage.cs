namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct DismountCompleteMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DismountComplete;
		public GameObject MountId { get; set; }
	}
}