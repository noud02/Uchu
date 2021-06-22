using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyRacingClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyRacingClient;
		[Default]
		public RacingClientNotificationType EventType { get; set; }
		public int Param1 { get; set; }
		public GameObject ParamObj { get; set; }
		[Wide]
		public string ParamStr { get; set; }
		public GameObject SingleClient { get; set; }
	}
}