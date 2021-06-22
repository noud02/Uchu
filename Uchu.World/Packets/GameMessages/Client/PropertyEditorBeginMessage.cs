using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PropertyEditorBeginMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PropertyEditorBegin;
		[Default]
		public int DistanceType { get; set; }
		[Default]
		public GameObject PropertyObjectId { get; set; }
		[Default(1)]
		public int StartMode { get; set; }
		[Default]
		public bool StartPaused { get; set; }
	}
}