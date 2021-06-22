using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestDieMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestDie;
		public bool Unknown { get; set; }
		[Wide]
		public string DeathType { get; set; }
		public float DirectionRelativeAngleXz { get; set; }
		public float DirectionRelativeAngleY { get; set; }
		public float DirectionRelativeForce { get; set; }
		[Default]
		public KillType KillType { get; set; }
		public GameObject KillerId { get; set; }
		public GameObject LootOwnerId { get; set; }
	}
}