using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct NotifyTamingBuildSuccessMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyTamingBuildSuccess;
		public Vector3 BuildPosition { get; set; }
	}
}