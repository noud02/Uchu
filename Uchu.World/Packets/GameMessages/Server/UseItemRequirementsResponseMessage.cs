namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UseItemRequirementsResponseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UseItemRequirementsResponse;
		public UseItemResponse UseResponse { get; set; }
	}
}