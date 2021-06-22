namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct OrientToObjectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.OrientToObject;
		public GameObject ObjId { get; set; }
	}
}