using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct GetLastCustomBuildMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.GetLastCustomBuild;
		[Wide]
		public string TokenizedLotList { get; set; }
	}
}