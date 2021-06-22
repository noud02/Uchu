using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ResetMissionsMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ResetMissions;
		[Default(-1)]
		public int MissionId { get; set; }
	}
}