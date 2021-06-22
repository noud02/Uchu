namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PetResponseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PetResponse;
		public GameObject Pet { get; set; }
		public int PetCommandType { get; set; }
		public int Response { get; set; }
		public int TypeId { get; set; }
	}
}