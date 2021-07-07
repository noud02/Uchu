using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World.Objects
{
    /// <summary>
    /// A set of items that when worn combined execute special skills
    /// </summary>
    /// <remarks>
    /// Most notable examples include the faction gear, see https://lu-explorer.web.app/objects/item-sets for a complete
    /// list.
    /// </remarks>
    public class ItemSet : Object
    {
        /// <summary>
        /// Creates an item set for the given inventory, tracking the inventory updates for it
        /// </summary>
        private ItemSet()
        {
            _equippedItemsInSet = new HashSet<Lot>();
            _skillSetMap = new Dictionary<int, int?>();
            _itemsInSet = new HashSet<Lot>();
        }

        /// <summary>
        /// Creates a new item set
        /// </summary>
        /// <param name="inventory">The inventory this item set should track</param>
        /// <param name="setId">The unique set id for this item set</param>
        /// <returns>An item set for the given parameters</returns>
        public static ItemSet Instantiate(InventoryComponent inventory, int setId)
        {
            var instance = Instantiate<ItemSet>(inventory.Zone);
            
            instance._setId = setId;
            instance._inventory = inventory;
            instance._inventory.ActiveItemSets.Add(instance);
            instance.SetupEvents();

            return instance;
        }

        /// <summary>
        /// Sets up all the events for the item set
        /// </summary>
        private void SetupEvents()
        {
            Listen(_inventory.OnEquipped, async item =>
            {
                if (_itemsInSet.Contains(item.Lot))
                {
                    _equippedItemsInSet.Add(item.Lot);
                    
                    // Equip the skill set if this amount of items should equip one
                    if (_skillSetMap.TryGetValue(_equippedItemsInSet.Count, out var skillSetId)
                        && skillSetId.HasValue
                        && _inventory.GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                        await skillComponent.EquipSkillSetAsync(skillSetId.Value);
                }
            });

            Listen(_inventory.OnUnEquipped, async item =>
            {
                if (_equippedItemsInSet.Contains(item.Lot))
                {
                    if (_skillSetMap.TryGetValue(_equippedItemsInSet.Count, out var skillSetId)
                        && skillSetId.HasValue
                        && _inventory.GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                        await skillComponent.UnequipSkillSetAsync(skillSetId.Value);
                    
                    _equippedItemsInSet.Remove(item.Lot);
                }
            });
        }
        
        /// <summary>
        /// The inventory this item set belongs to
        /// </summary>
        private InventoryComponent _inventory;
        
        /// <summary>
        /// Unique Id of the set
        /// </summary>
        private int _setId;
        
        /// <summary>
        /// The items in the inventory that are equipped and part of this item set
        /// </summary>
        private readonly HashSet<Lot> _equippedItemsInSet;
        
        /// <summary>
        /// All the possible items that can be part of this item set
        /// </summary>
        private readonly HashSet<Lot> _itemsInSet;
        
        /// <summary>
        /// Maps the number of required items to the skill that should execute
        /// </summary>
        private readonly Dictionary<int, int?> _skillSetMap;

        /// <summary>
        /// For an item makes sure that an item set is created if said item is part of one, if this item is not port of
        /// an item set or the the item set this item belongs to is already created, this does nothing
        /// </summary>
        /// <param name="inventory">The inventory to get possible set items from</param>
        /// <param name="lot">The lot for which we wish to check if item sets should be tracked</param>
        public static async Task CreateIfNewSet(InventoryComponent inventory, Lot lot)
        {
            await using var cdClient = new CdClientContext();
            var clientItemSet = cdClient.ItemSetsTable.FirstOrDefault(
                i => i.ItemIDs.Contains(lot.ToString()));
            
            // Make sure that the item set is valid and that the inventory doesn't already track an item set with this ID
            if (clientItemSet?.SetID == null 
                || inventory.ActiveItemSets.Any(i => i._setId == clientItemSet.SetID.Value))
                return;
            
            var itemSet = Instantiate(inventory, clientItemSet.SetID.Value);
            Start(itemSet);
            
            foreach (var itemSetLot in clientItemSet.ItemIDs.Split(","))
            {
                itemSet._itemsInSet.Add(new Lot(int.Parse(itemSetLot)));
            }
            
            // The skill sets that are unlocked when wearing n items of a set
            itemSet._skillSetMap[2] = clientItemSet.SkillSetWith2;
            itemSet._skillSetMap[3] = clientItemSet.SkillSetWith3;
            itemSet._skillSetMap[4] = clientItemSet.SkillSetWith4;
            itemSet._skillSetMap[5] = clientItemSet.SkillSetWith5;
            itemSet._skillSetMap[6] = clientItemSet.SkillSetWith6;
        }
    }
}