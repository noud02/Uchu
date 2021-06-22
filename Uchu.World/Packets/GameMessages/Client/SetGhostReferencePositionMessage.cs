using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetGhostReferencePositionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetGhostReferencePosition;
		public Vector3 Position { get; set; }
	}
}