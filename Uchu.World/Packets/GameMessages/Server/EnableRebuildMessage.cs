using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct EnableRebuildMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.EnableRebuild;
		public bool Enable { get; set; }
		public bool IsFail { get; set; }
		public bool IsSuccess { get; set; }
		[Default]
		public RebuildFailReason FailReason { get; set; }
		public float Duration { get; set; }
		public Player Player { get; set; }
	}
}