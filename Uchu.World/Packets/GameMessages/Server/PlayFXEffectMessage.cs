using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayFXEffectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayFXEffect;
		[Default(-1)]
		public int EffectId { get; set; }
		[Wide]
		public string EffectType { get; set; }
		[Default(1)]
		public float Scale { get; set; }
		public string Name { get; set; }
		[Default(1)]
		public float Priority { get; set; }
		[Default]
		public GameObject Secondary { get; set; }
		public bool Serialize { get; set; }
	}
}