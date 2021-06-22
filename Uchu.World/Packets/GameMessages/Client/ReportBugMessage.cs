using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ReportBugMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ReportBug;
		[Wide]
		public string Body { get; set; }
		public string ClientVersion { get; set; }
		public string OtherPlayerId { get; set; }
		public string Selection { get; set; }
	}
}