namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PropertyRentalResponseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PropertyRentalResponse;
		public int CloneId { get; set; }
		public PropertyRentalResponseCode Code { get; set; }
		public GameObject PropertyId { get; set; }
		public long Rentdue { get; set; }
	}
}