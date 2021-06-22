using System.Numerics;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct KnockbackMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.Knockback;
		[Default]
		public GameObject Caster { get; set; }
		[Default]
		public GameObject Originator { get; set; }
		[Default]
		public int KnockbackTime { get; set; }
		public Vector3 Vector { get; set; }
	}
}