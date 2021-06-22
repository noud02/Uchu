using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TransferToLastNonInstanceMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TransferToLastNonInstance;
		public bool UseLastPosition { get; set; }
		public GameObject PlayerId { get; set; }
		[Default]
		public Vector3 Position { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
	}
}