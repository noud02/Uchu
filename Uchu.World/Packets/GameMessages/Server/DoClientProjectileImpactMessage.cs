using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct DoClientProjectileImpactMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DoClientProjectileImpact;
		[Default]
		public long ProjectileId { get; set; }
		[Default]
		public GameObject Owner { get; set; }
		[Default]
		public GameObject Target { get; set; }
		public byte[] Data { get; set; }
	}
}