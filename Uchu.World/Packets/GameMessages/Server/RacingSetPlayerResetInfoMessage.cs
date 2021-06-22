using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RacingSetPlayerResetInfoMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RacingSetPlayerResetInfo;
		public int CurrentLap { get; set; }
		public uint FurthestResetPlane { get; set; }
		public GameObject PlayerId { get; set; }
		public Vector3 RespawnPos { get; set; }
		public uint UpcomingPlane { get; set; }
	}
}