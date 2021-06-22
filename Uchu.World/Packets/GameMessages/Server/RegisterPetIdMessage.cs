namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RegisterPetIdMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RegisterPetId;
		public GameObject Pet { get; set; }
	}
}