using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct TeamRemovePlayerMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.TeamRemovePlayer;
		public bool Disband { get; set; }
		public bool IsKicked { get; set; }
		public bool IsLeaving { get; set; }
		public bool Local { get; set; }
		public GameObject LeaderId { get; set; }
		public GameObject PlayerId { get; set; }
		[Wide]
		public string Name { get; set; }
	}
}