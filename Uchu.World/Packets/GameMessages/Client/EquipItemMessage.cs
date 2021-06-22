namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct EquipItemMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.EquipInventory;
        public bool IgnoreCooldown { get; set; }
        public bool OutSuccess { get; set; }
        public Item Item { get; set; }
    }
}