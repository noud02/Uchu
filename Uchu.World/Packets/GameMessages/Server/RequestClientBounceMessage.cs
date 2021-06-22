using System.Numerics;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct RequestClientBounceMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.RequestClientBounce;
		public GameObject BounceTargetId { get; set; }
		public Vector3 BounceTargetPosOnServer { get; set; }
		public Vector3 BouncedObjLinVel { get; set; }
		public GameObject RequestSourceId { get; set; }
		public bool AllBounced { get; set; }
		public bool AllowClientOverride { get; set; }
	}
}