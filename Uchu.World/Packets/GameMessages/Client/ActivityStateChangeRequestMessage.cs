using Uchu.Core;

namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct ActivityStateChangeRequestMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.ActivityStateChangeRequest;
		public GameObject ObjId { get; set; }
		public int NumValue1 { get; set; }
		public int NumValue2 { get; set; }
		[Wide]
		public string StringValue { get; set; }
	}
}