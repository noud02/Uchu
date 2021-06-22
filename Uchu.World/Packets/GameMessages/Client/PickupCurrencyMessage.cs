using System.Numerics;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PickupCurrencyMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PickupCurrency;
		public uint Currency { get; set; }
		public Vector3 Position { get; set; }
	}
}