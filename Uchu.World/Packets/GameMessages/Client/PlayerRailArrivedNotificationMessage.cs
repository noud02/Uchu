using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PlayerRailArrivedNotificationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayerRailArrivedNotification;
		[Wide]
		public string PathName { get; set; }
		public int WaypointNumber { get; set; }
	}
}