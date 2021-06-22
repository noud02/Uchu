using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetJetPackModeMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetJetPackMode;
		public bool BypassChecks { get; set; }
		public bool DoHover { get; set; }
		public bool Use { get; set; }
		[Default(-1)]
		public int EffectId { get; set; }
		[Default(10)]
		public float AirSpeed { get; set; }
		[Default(15)]
		public float MaxAirSpeed { get; set; }
		[Default]
		public float VerticalVelocity { get; set; }
		[Default(-1)]
		public int WarningEffectId { get; set; }
	}
}