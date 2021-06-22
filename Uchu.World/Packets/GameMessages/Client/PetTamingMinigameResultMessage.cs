namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PetTamingMinigameResultMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PetTamingMinigameResult;
		public bool Success { get; set; }
	}
}