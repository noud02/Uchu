namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ShowPetActionButtonMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ShowPetActionButton;
		public PetAbilityType ButtonLabel { get; set; }
		public bool Show { get; set; }
	}
}