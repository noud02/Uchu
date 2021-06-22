using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TransferToZoneCheckedIMMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TransferToZoneCheckedIM;
		public bool IsThereAQueue { get; set; }
		[Default]
		public int CloneId { get; set; }
		[Default]
		public Vector3 Position { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
		[Wide]
		public string SpawnPoint { get; set; }
		public ushort UcInstanceType { get; set; }
		[Default]
		public ZoneId ZoneId { get; set; }
	}
}