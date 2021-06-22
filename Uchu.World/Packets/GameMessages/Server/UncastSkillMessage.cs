namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UncastSkillMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UncastSkill;
		public int SkillId { get; set; }
	}
}