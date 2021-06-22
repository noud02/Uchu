using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyPetTamingMinigameMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyPetTamingMinigame;
		public ObjectId PetId { get; set; }
		public Player PlayerTaming { get; set; }
		public bool ForceTeleport { get; set; }
		public PetTamingNotifyType NotifyType { get; set; }
		public Vector3 PetDestinationPosition { get; set; }
		public Vector3 TeleportPosition { get; set; }
		[Default]
		[NiQuaternion]
		public Quaternion TeleportRotation { get; set; }
	}
}