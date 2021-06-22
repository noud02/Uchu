using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyClientShootingGalleryScoreMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyClientShootingGalleryScore;
		public float AddTime { get; set; }
		public int Score { get; set; }
		public GameObject Target { get; set; }
		public Vector3 TargetPos { get; set; }
	}
}