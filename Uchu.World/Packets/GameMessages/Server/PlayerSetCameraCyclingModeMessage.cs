using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayerSetCameraCyclingModeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayerSetCameraCyclingMode;
		public bool AllowCyclingWhileDeadOnly { get; set; }
		[Default]
		public CyclingMode CyclingMode { get; set; }
	}
}