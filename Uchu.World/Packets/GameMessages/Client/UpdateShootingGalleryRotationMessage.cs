using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct UpdateShootingGalleryRotationMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.UpdateShootingGalleryRotation;
		public float Angle { get; set; }
		public Vector3 Facing { get; set; }
		public Vector3 MuzzlePos { get; set; }
	}
}