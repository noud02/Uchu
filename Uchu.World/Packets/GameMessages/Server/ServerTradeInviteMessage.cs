using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ServerTradeInviteMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ServerTradeInvite;
		public bool NeedInvitePopUp { get; set; }
		public GameObject Requestor { get; set; }
		[Wide]
		public string Name { get; set; }
	}
}