using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct CommandPetMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.CommandPet;
		public Vector3 GenericPosInfo { get; set; }
		public GameObject ObjIdSource { get; set; }
		public int PetCommandType { get; set; }
		public int TypeId { get; set; }
		public bool OverrideObey { get; set; }
	}
}