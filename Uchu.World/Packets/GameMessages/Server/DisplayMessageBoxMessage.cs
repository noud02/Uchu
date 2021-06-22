using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct DisplayMessageBoxMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DisplayMessageBox;
		public bool Show { get; set; }
		public GameObject CallbackClient { get; set; }
		[Wide]
		public string Identifier { get; set; }
		public int ImageId { get; set; }
		[Wide]
		public string Text { get; set; }
		[Wide]
		public string UserData { get; set; }
	}
}