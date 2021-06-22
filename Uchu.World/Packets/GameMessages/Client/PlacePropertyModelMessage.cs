namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PlacePropertyModelMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlacePropertyModel;
		public GameObject ModelId { get; set; }
	}
}