using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyClientObjectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyClientObject;
		[Wide]
		public string Name { get; set; }
		public int Param1 { get; set; }
		public int Param2 { get; set; }
		public GameObject ParamObj { get; set; }
		public string ParamStr { get; set; }
	}
}