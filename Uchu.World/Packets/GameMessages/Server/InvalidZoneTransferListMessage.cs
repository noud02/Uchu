using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct InvalidZoneTransferListMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.InvalidZoneTransferList;
		[Wide]
		public string CustomerFeedbackUrl { get; set; }
		[Wide]
		public string InvalidMapTransferList { get; set; }
		public bool CustomerFeedbackOnExit { get; set; }
		public bool CustomerFeedbackOnInvalidMapTransfer { get; set; }
	}
}