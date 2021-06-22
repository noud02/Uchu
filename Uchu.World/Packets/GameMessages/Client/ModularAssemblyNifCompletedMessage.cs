namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ModularAssemblyNifCompletedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ModularAssemblyNIFCompleted;
		public GameObject ObjectId { get; set; }
	}
}