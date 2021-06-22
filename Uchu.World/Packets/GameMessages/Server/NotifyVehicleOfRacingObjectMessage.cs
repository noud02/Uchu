using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct NotifyVehicleOfRacingObjectMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.NotifyVehicleOfRacingObject;
		[Default]
		public GameObject RacingObjectId { get; set; }
	}
}