using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct EchoSyncSkillMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.EchoSyncSkill;
		public bool Done { get; set; }
		public byte[] Content { get; set; }
		public uint BehaviorHandle { get; set; }
		public uint SkillHandle { get; set; }
	}
}