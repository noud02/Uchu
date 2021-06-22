using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ShowActivityCountdownMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ShowActivityCountdown;
		public bool PlayAdditionalSound { get; set; }
		public bool PlayCountdownSound { get; set; }
		[Wide]
		public string SoundName { get; set; }
		public int StateToPlaySoundOn { get; set; }
	}
}