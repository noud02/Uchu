using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlaceModelResponseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlaceModelResponse;
		[Default]
		public Vector3 Position { get; set; }
		[Default]
		public GameObject PropertyPlaqueId { get; set; }
		[Default]
		public int Response { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion Rotation { get; set; }
	}
}