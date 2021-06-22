using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PetNameChangedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PetNameChanged;
		public PetModerationStatus ModerationStatus { get; set; }
		[Wide]
		public string Name { get; set; }
		[Wide]
		public string OwnerName { get; set; }
	}
}