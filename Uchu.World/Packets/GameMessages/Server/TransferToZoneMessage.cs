using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TransferToZoneMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TransferToZone;
		public bool CheckTransferAllowed { get; set; }
		[Default]
		public int CloneId { get; set; }
		[Default]
		public Vector3 Position { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
		[Wide]
		public string SpawnPoint { get; set; }
		public ushort InstanceType { get; set; }
		[Default]
		public ZoneId ZoneId { get; set; }
	}
}