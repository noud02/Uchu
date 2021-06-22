using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestServerProjectileImpactMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestServerProjectileImpact;
		[Default]
		public GameObject Projectile { get; set; }
		[Default]
		public GameObject Target { get; set; }
		public byte[] Data { get; set; }
	}
}