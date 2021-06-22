using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct VehicleNotifyHitImaginationServerMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VehicleNotifyHitImaginationServer;
		[Default]
		public GameObject PickupObjId { get; set; }
		[Default]
		public GameObject PickupSpawnerId { get; set; }
		[Default(-1)]
		public int PickupSpawnerIndex { get; set; }
		[Default]
		public Vector3 VehiclePosition { get; set; }
	}
}