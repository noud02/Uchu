namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct StopFXEffectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StopFXEffect;
		public bool KillImmediate { get; set; }
		public string Name { get; set; }
	}
}