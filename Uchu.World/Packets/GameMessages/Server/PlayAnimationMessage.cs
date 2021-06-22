using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayAnimationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayAnimation;
		[Wide]
		public string AnimationId { get; set; }
		public bool ExpectAnimToExist { get; set; }
		public bool PlayImmediate { get; set; }
		public bool TriggerOnCompleteMessage { get; set; }
		[Default(0.4f)]
		public float Priority { get; set; }
		[Default(1)]
		public float Scale { get; set; }
	}
}