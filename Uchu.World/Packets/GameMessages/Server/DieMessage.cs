using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct DieMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.Die;
		public bool ClientDeath { get; set; }
		public bool SpawnLoot { get; set; }
		[Wide]
		public string DeathType { get; set; }
		public float DirectionRelativeAngleXz { get; set; }
		public float DirectionRelativeAngleY { get; set; }
		public float DirectionRelativeForce { get; set; }
		[Default]
		public KillType KillType { get; set; }
		public GameObject Killer { get; set; }
		[Default]
		public GameObject LootOwner { get; set; }
	}
}