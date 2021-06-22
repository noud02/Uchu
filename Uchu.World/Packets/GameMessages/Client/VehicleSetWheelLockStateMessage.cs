using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct VehicleSetWheelLockStateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VehicleSetWheelLockState;
		public bool ExtraFriction { get; set; }
		public bool Locked { get; set; }
	}
}