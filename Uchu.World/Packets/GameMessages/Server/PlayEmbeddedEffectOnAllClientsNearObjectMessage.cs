using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct PlayEmbeddedEffectOnAllClientsNearObjectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PlayEmbeddedEffectOnAllClientsNearObject;
		[Wide]
		public string EffectName { get; set; }
		public GameObject FromObject { get; set; }
		public float Radius { get; set; }
	}
}