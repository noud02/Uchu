using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ModifyGhostingDistanceMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ModifyGhostingDistance;
		[Default(1)]
		public float Distance { get; set; }
	}
}