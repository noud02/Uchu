namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RegisterPetDBIdMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RegisterPetDBId;
		public GameObject Pet { get; set; }
	}
}