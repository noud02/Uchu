using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetPetNameModeratedMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetPetNameModerated;
		[Default]
		public GameObject PetDbId { get; set; }
		public PetModerationStatus ModerationStatus { get; set; }
	}
}