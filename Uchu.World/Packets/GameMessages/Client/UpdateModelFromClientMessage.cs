using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct UpdateModelFromClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UpdateModelFromClient;
		public GameObject ModelId { get; set; }
		public Vector3 Position { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
	}
}