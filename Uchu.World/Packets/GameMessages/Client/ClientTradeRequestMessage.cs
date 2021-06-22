using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ClientTradeRequestMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ClientTradeRequest;
		public bool NeedInvitePopUp { get; set; }
		public GameObject Invitee { get; set; }
	}
}