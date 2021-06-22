using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ModifyLegoScoreMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ModifyLegoScore;
		public long Score { get; set; }
		[Default]
		public LootType SourceType { get; set; }
	}
}