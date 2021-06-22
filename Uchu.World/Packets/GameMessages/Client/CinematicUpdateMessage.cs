using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct CinematicUpdateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.CinematicUpdate;
		[Default]
		public CinematicEvent Event { get; set; }
		[Default(-1)]
		public float OverallTime { get; set; }
		[Wide]
		public string PathName { get; set; }
		[Default(-1)]
		public float PathTime { get; set; }
		[Default(-1)]
		public int Waypoint { get; set; }
	}
}