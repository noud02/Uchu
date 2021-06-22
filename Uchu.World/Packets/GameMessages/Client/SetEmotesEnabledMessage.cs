using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetEmotesEnabledMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetEmotesEnabled;
		public bool EnableEmotes { get; set; }
	}
}