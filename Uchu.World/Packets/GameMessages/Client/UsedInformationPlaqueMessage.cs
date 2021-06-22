namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct UsedInformationPlaqueMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UsedInformationPlaque;
		public GameObject Plaque { get; set; }
	}
}