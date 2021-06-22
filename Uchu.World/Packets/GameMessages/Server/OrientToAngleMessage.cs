namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct OrientToAngleMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.OrientToAngle;
		public bool RelativeToCurrent { get; set; }
		public float Angle { get; set; }
	}
}