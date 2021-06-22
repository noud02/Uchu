using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetRailMovementMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetRailMovement;
		public bool PathGoForward { get; set; }
		[Wide]
		public string PathName { get; set; }
		public uint PathStart { get; set; }
		[Default(-1)]
		public int RailActivatorComponentId { get; set; }
		[Default]
		public GameObject RailActivatorObjId { get; set; }
	}
}