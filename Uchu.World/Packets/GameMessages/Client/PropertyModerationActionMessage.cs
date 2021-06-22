using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PropertyModerationActionMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PropertyModerationAction;
		[Default]
		public GameObject CharacterId { get; set; }
		[Wide]
		public string Info { get; set; }
		[Default(-1)]
		public int NewModerationStatus { get; set; }
	}
}