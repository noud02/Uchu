using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct BuildModeSetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BuildModeSet;
		public bool Start { get; set; }
		[Default(-1)]
		public int DistanceType { get; set; }
		public bool ModePaused { get; set; }
		[Default(1)]
		public int ModeValue { get; set; }
		public GameObject PlayerId { get; set; }
		[Default]
		public Vector3 StartPos { get; set; }
	}
}