using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetStunImmunityMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetStunImmunity;
		[Default]
		public GameObject Caster { get; set; }
		public ImmunityState StateChangeType { get; set; }
		public bool ImmuneToStunAttack { get; set; }
		public bool ImmuneToStunEquip { get; set; }
		public bool ImmuneToStunInteract { get; set; }
		public bool ImmuneToStunJump { get; set; }
		public bool ImmuneToStunMove { get; set; }
		public bool ImmuneToStunTurn { get; set; }
		public bool ImmuneToStunUseItem { get; set; }
	}
}