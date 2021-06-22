namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ObjectActivatedClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ObjectActivatedClient;
		public GameObject ActivatorId { get; set; }
		public GameObject ObjectActivatedId { get; set; }
	}
}