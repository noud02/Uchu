using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetPetNameMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetPetName;
		[Wide]
		public string Name { get; set; }
		[Default]
		public GameObject PetDbId { get; set; }
	}
}