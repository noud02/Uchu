using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayerReachedRespawnCheckpointMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayerReachedRespawnCheckpoint;
		public Vector3 Position { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
	}
}