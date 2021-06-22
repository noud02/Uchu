using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct UpdatePropertyModelCountMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UpdatePropertyModelCount;
		[Default]
		public uint ModelCount { get; set; }
	}
}