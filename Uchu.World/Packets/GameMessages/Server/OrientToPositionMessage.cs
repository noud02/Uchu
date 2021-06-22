using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct OrientToPositionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.OrientToPosition;
		public Vector3 Position { get; set; }
	}
}