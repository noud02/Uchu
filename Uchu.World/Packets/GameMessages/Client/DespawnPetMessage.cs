namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct DespawnPetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DespawnPet;
		public bool DeletePet { get; set; }
	}
}