namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetStatusImmunityMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetStatusImmunity;
		public ImmunityState State { get; set; }
		public bool ImmuneToBasicAttack { get; set; }
		public bool ImmuneToDOT { get; set; }
		public bool ImmuneToImaginationGain { get; set; }
		public bool ImmuneToImaginationLoss { get; set; }
		public bool ImmuneToInterrupt { get; set; }
		public bool ImmuneToKnockback { get; set; }
		public bool ImmuneToPullToPoint { get; set; }
		public bool ImmuneToQuickbuildInterrupt { get; set; }
		public bool ImmuneToSpeed { get; set; }
	}
}