using InfectedRose.Lvl;
using Uchu.Core;

namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct DisplayTooltipMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.DisplayTooltip;
		public bool DoOrDie { get; set; }
		public bool NoRepeat { get; set; }
		public bool NoRevive { get; set; }
		public bool IsPropertyTooltip { get; set; }
		public bool Show { get; set; }
		public bool Translate { get; set; }
		public int Time { get; set; }
		[Wide]
		public string Id { get; set; }
		public LegoDataDictionary LocalizeParams { get; set; }
		[Wide]
		public string ImageName { get; set; }
		[Wide]
		public string Text { get; set; }
	}
}