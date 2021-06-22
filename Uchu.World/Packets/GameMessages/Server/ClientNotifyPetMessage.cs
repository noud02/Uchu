namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ClientNotifyPetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ClientNotifyPet;
		public GameObject ObjIdSource { get; set; }
		public PetNotificationType PetNotificationType { get; set; }
	}
}