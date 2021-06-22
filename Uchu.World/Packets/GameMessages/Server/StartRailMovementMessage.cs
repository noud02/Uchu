using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct StartRailMovementMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StartRailMovement;
		public bool DamageImmune { get; set; }
		public bool NoAggro { get; set; }
		public bool NotifyActivator { get; set; }
		public bool ShowNameBillboard { get; set; }
		public bool CameraLocked { get; set; }
		public bool CollisionEnabled { get; set; }
		[Wide]
		public string LoopSound { get; set; }
		public bool PathGoForward { get; set; }
		[Wide]
		public string PathName { get; set; }
		[Default]
		public uint PathStart { get; set; }
		[Default(-1)]
		public int RailActivatorComponentId { get; set; }
		[Default]
		public GameObject RailActivatorObjId { get; set; }
		[Wide]
		public string StartSound { get; set; }
		[Wide]
		public string StopSound { get; set; }
		public bool UseDb { get; set; }
	}
}