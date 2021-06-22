using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct StartCelebrationEffectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.StartCelebrationEffect;
		[Wide]
		public string Animation { get; set; }
		[Default(11164)]
		public Lot BackgroundObject { get; set; }
		[Default(12458)]
		public Lot CameraPathLot { get; set; }
		[Default(1)]
		public float CeleLeadIn { get; set; }
		[Default(0.8)]
		public float CeleLeadOut { get; set; }
		[Default(-1)]
		public int CelebrationId { get; set; }
		public float Duration { get; set; }
		public uint IconId { get; set; }
		[Wide]
		public string MainText { get; set; }
		public string MixerProgram { get; set; }
		public string MusicCue { get; set; }
		public string PathNodeName { get; set; }
		public string SoundGuid { get; set; }
		[Wide]
		public string SubText { get; set; }
	}
}