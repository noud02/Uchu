using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeleportMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.Teleport;
		public bool IgnoreY { get; set; }
		public bool SetRotation { get; set; }
		public bool SkipAllChecks { get; set; }
		public Vector3 Position { get; set; }
		public bool UseNavmesh { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
	}
}