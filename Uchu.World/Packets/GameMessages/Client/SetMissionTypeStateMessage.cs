using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetMissionTypeStateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetMissionTypeState;
		[Default(MissionLockState.New)]
		public MissionLockState LockState { get; set; }
		public string SubType { get; set; }
		public string Type { get; set; }
	}
}