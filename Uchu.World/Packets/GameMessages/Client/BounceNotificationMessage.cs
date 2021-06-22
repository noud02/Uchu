namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BounceNotificationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BounceNotification;
		public GameObject ObjIdBounced { get; set; }
		public GameObject ObjIdBouncer { get; set; }
		public bool Success { get; set; }
	}
}