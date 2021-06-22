using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetModelToBuildMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetModelToBuild;
		[Default]
		public Lot TemplateId { get; set; }
	}
}