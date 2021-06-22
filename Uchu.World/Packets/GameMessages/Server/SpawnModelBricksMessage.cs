using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SpawnModelBricksMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SpawnModelBricks;
		[Default]
		public float Amount { get; set; }
		[Default]
		public Vector3 Pos { get; set; }
	}
}