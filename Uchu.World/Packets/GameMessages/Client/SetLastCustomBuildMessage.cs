using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct SetLastCustomBuildMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetLastCustomBuild;
		[Wide]
		public string Tokens { get; set; }
	}
}