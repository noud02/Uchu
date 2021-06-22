namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct FetchModelMetadataRequestMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.FetchModelMetadataRequest;
		public int Context { get; set; }
		public GameObject ObjectId { get; set; }
		public GameObject RequestorId { get; set; }
		public GameObject UgId { get; set; }
	}
}