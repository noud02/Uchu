namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ControlBehaviorsMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ControlBehaviors;
		public byte[] Args { get; set; }
		public string Command { get; set; }
	}
}