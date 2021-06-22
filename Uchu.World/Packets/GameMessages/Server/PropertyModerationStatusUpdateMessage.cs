using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PropertyModerationStatusUpdateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PropertyModerationStatusUpdate;
		[Default(-1)]
		public int NewModerationStatus { get; set; }
		[Wide]
		public string RejectionReason { get; set; }
	}
}