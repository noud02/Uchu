using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ServerTradeFinalReplyMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ServerTradeFinalReply;
		public bool Result { get; set; }
		public GameObject Invitee { get; set; }
		[Wide]
		public string Name { get; set; }
	}
}