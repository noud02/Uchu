using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct VehicleLockInputMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VehicleLockInput;
		public bool LockWheels { get; set; }
		public bool LockedPowerslide { get; set; }
		[Default]
		public float LockedX { get; set; }
		[Default]
		public float LockedY { get; set; }
	}
}