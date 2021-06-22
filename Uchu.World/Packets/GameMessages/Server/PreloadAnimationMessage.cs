using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PreloadAnimationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PreloadAnimation;
		[Wide]
		public string AnimationId { get; set; }
		public bool Handled { get; set; }
		public GameObject RespondObjId { get; set; }
		public LegoDataDictionary UserData { get; set; }
	}
}