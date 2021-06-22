using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct AddSkillMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.AddSkill;
		[Default]
		public int AiCombatWeight { get; set; }
		public bool FromSkillSet { get; set; }
		[Default]
		public SkillCastType CastType { get; set; }
		[Default(-1)]
		public float TimeSecs { get; set; }
		[Default(-1)]
		public int TimesCanCast { get; set; }
		public uint SkillId { get; set; }
		[Default(-1)]
		public BehaviorSlot SlotId { get; set; }
		public bool Temporary { get; set; }
	}
}