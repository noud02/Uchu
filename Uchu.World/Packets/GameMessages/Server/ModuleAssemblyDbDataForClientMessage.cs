using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ModuleAssemblyDBDataForClientMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ModuleAssemblyDBDataForClient;
		public GameObject AssemblyId { get; set; }
		[Wide]
		public string Blob { get; set; }
	}
}