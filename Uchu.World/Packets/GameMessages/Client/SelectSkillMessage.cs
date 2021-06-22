using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SelectSkillMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SelectSkill;
		public bool FromSkillSet { get; set; }
		public int SkillId { get; set; }
	}
}