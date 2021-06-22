using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SyncSkillMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SyncSkill;
		public bool Done { get; set; }
		public byte[] Content { get; set; }
		public uint BehaviorHandle { get; set; }
		public uint SkillHandle { get; set; }
	}
}