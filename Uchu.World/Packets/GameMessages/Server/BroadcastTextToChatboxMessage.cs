using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct BroadcastTextToChatboxMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.BroadcastTextToChatbox;
		public LegoDataDictionary Attrs { get; set; }
		[Wide]
		public string Text { get; set; }
	}
}