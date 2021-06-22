namespace Uchu.World
{
	[ServerGameMessagePacketStruct]
	public struct VendorTransactionResultMessage
	{
		public GameObject Associate { get; set; }
		public GameMessageId GameMessageId => GameMessageId.VendorTransactionResult;
		public TransactionResult Result { get; set; }
	}
}