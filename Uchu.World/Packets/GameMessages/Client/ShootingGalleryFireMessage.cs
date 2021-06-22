using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ShootingGalleryFireMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ShootingGalleryFire;
		public Vector3 TargetPos { get; set; }
		public float W { get; set; }
		public float X { get; set; }
		public float Y { get; set; }
		public float Z { get; set; }
	}
}