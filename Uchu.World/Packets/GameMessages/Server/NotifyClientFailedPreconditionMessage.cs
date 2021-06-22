using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyClientFailedPreconditionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyClientFailedPrecondition;
		[Wide]
		public string Reason { get; set; }
		public int Id { get; set; }
	}
}