using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlatformResyncMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlatformResync;
		public bool Reverse { get; set; }
		public bool StopAtDesiredWaypoint { get; set; }
		public int Command { get; set; }
		public PlatformState State { get; set; }
		public int UnexpectedCommand { get; set; }
		public float IdleTimeElapsed { get; set; }
		public float MoveTimeElapsed { get; set; }
		public float PercentBetweenPoints { get; set; }
		public int DesiredWaypointIndex { get; set; }
		public int Index { get; set; }
		public int NextIndex { get; set; }
		public Vector3 UnexpectedLocation { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion UnexpectedRotation { get; set; }
	}
}