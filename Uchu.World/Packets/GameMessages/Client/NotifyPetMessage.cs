namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct NotifyPetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyPet;
		public GameObject ObjIdSource { get; set; }
		public GameObject ObjToNotifyPetAbout { get; set; }
		public PetNotificationType PetNotificationType { get; set; }
	}
}