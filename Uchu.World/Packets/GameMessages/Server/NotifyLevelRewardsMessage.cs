using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyLevelRewardsMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyLevelRewards;
		public int Level { get; set; }
		public bool SendingRewards { get; set; }
	}
}