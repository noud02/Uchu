using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct RequestUseMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestUse;
		public bool IsMultiInteract { get; set; }
		public uint MultiInteractId { get; set; }
		public InteractionType MultiInteractType { get; set; }
		public GameObject TargetObject { get; set; }
		public bool Secondary { get; set; }
	}
}