using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ParseChatMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ParseChatMessage;
		public int ClientState { get; set; }
		[Wide]
		public string Message { get; set; }
	}
}