using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct SetResurrectRestoreValuesMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.SetResurrectRestoreValues;
		[Default(-1)]
		public int ArmorRestore { get; set; }
		[Default(-1)]
		public int HealthRestore { get; set; }
		[Default(-1)]
		public int ImaginationRestore { get; set; }
	}
}