namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct LockNodeRotationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.LockNodeRotation;
		public string NodeName { get; set; }
	}
}