using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UpdatePlayerStatisticMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UpdatePlayerStatistic;
		public int UpdateId { get; set; }
		[Default(1)]
		public long UpdateValue { get; set; }
	}
}