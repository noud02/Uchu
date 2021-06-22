using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct CasterDeadMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.CasterDead;
		[Default]
		public GameObject Caster { get; set; }
		[Default]
		public uint SkillHandle { get; set; }
	}
}