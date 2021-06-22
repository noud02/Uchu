using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct MessageBoxRespondMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.MessageBoxRespond;
		public int Button { get; set; }
		[Wide]
		public string Identifier { get; set; }
		[Wide]
		public string UserData { get; set; }
	}
}