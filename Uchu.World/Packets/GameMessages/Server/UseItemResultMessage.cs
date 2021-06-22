using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UseItemResultMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UseItemResult;
		public Lot ItemTemplateId { get; set; }
		public bool UseItemResult { get; set; }
	}
}