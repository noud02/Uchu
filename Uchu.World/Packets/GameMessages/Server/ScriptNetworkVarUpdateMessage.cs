using InfectedRose.Lvl;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct ScriptNetworkVarUpdateMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ScriptNetworkVarUpdate;
		public LegoDataDictionary Data { get; set; }
	}
}