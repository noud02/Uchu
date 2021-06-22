using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct UpdatePropertyPerformanceCostMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UpdatePropertyPerformanceCost;
		[Default]
		public float PerformanceCost { get; set; }
	}
}