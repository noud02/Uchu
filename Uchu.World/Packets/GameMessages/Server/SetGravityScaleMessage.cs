namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetGravityScaleMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetGravityScale;
		public float Scale { get; set; }
	}
}