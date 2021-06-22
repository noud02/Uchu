using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct DeleteModelFromClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DeleteModelFromClient;
		[Default]
		public GameObject ModelId { get; set; }
		[Default]
		public DeleteReason Reason { get; set; }
	}
}