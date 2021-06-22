using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ChoiceBoxRespondMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ChoiceBoxRespond;
		[Wide]
		public string ButtonIdentifier { get; set; }
		public int Button { get; set; }
		[Wide]
		public string Identifier { get; set; }
	}
}