namespace Uchu.World
{
    [ClientGameMessagePacketStruct]
    public struct UnEquipItemMessage
    {
        public GameObject Associate { get; set; }
        public GameMessageId GameMessageId => GameMessageId.UnEquipInventory;
        public bool EvenIfDead { get; set; }
        public bool IgnoreCooldown { get; set; }
        public bool OutSuccess { get; set; }
        public Item ItemToUnEquip { get; set; }
        public Item ReplacementItem { get; set; }
    }
}