namespace Uchu.World
{
	[ClientGameMessagePacketStruct]
	public struct PropertyEntranceSyncMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.PropertyEntranceSync;
		public bool IncludeNullAddress { get; set; }
		public bool IncludeNullDescription { get; set; }
		public bool PlayersOwn { get; set; }
		public bool UpdateUi { get; set; }
		public int NumResults { get; set; }
		public int ReputationTime { get; set; }
		public int SortMethod { get; set; }
		public int StartIndex { get; set; }
		public string FilterText { get; set; }
	}
}