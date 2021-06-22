namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetPlayerAllowedRespawnMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetPlayerAllowedRespawn;
		public bool DontPromptForRespawn { get; set; }
	}
}