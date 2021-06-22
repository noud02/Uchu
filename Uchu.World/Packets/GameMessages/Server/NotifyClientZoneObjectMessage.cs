using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyClientZoneObjectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyClientZoneObject;
		[Wide]
		public string Name { get; set; }
		public int Param1 { get; set; }
		public int Param2 { get; set; }
		public GameObject ParamObj { get; set; }
		public string ParamStr { get; set; }
	}
}