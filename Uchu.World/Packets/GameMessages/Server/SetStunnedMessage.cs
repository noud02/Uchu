using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetStunnedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetStunned;
		[Default]
		public GameObject Originator { get; set; }
		public StunState StateChangeType { get; set; }
		public bool CantAttack { get; set; }
		public bool CantAttackOutChangeWasApplied { get; set; }
		public bool CantEquip { get; set; }
		public bool CantEquipOutChangeWasApplied { get; set; }
		public bool CantInteract { get; set; }
		public bool CantInteractOutChangeWasApplied { get; set; }
		public bool CantJump { get; set; }
		public bool CantJumpOutChangeWasApplied { get; set; }
		public bool CantMove { get; set; }
		public bool CantMoveOutChangeWasApplied { get; set; }
		public bool CantTurn { get; set; }
		public bool CantTurnOutChangeWasApplied { get; set; }
		public bool CantUseItem { get; set; }
		public bool CantUseItemOutChangeWasApplied { get; set; }
		public bool DontTerminateInteract { get; set; }
		public bool IgnoreImmunity { get; set; }
	}
}