using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ServerTradeInitialReplyMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ServerTradeInitialReply;
		public GameObject Invitee { get; set; }
		public ResultType ResultType { get; set; }
		[Wide]
		public string Name { get; set; }
	}
}