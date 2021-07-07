using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;
using Uchu.World.Objects;

namespace Uchu.World
{
    /// <summary>
    /// Represents the equipped inventory of a GameObject, not to be confused with the <see cref="InventoryManagerComponent"/>
    /// which represents the entire inventory of a GameObject. Also note that <see cref="InventoryComponent"/> is a
    /// replica component that was available in live while <see cref="InventoryManagerComponent"/> is a new component
    /// for efficient handling of the entire inventory of a game object.
    /// </summary>
    public class InventoryComponent : ReplicaComponent
    {
        /// <summary>
        /// Component Id of this component
        /// </summary>
        public override ComponentId Id => ComponentId.InventoryComponent;
        
        /// <summary>
        /// Mapping of items and their equipped slots on the game object
        /// </summary>
        private Dictionary<EquipLocation, Item> Items { get; }

        /// <summary>
        /// The items currently equipped in the inventory
        /// </summary>
        public Item[] EquippedItems => Items.Values.ToArray();

        /// <summary>
        /// Called when a new item is equipped
        /// </summary>
        public Event<Item> OnEquipped { get; }
        
        /// <summary>
        /// Called when an item is unequipped
        /// </summary>
        public Event<Item> OnUnEquipped { get; }
        
        /// <summary>
        /// The item sets currently active on the player
        /// </summary>
        public List<ItemSet> ActiveItemSets { get; }

        protected InventoryComponent()
        {
            Items = new Dictionary<EquipLocation, Item>();
            OnEquipped = new Event<Item>();
            OnUnEquipped = new Event<Item>();
            ActiveItemSets = new List<ItemSet>();

            Listen(OnDestroyed, () =>
            {
                OnEquipped.Clear();
                OnUnEquipped.Clear();
            });
            
            Listen(OnStart, async () =>
            {
                await LoadAsync();
            });
        }

        /// <summary>
        /// Loads the inventory for the game object, only for non-player GameObjects does this automatically equip the
        /// items as the component cannot know when the inventory manager component will be available to load the
        /// items from.
        /// </summary>
        private async Task LoadAsync()
        {
            if (!(GameObject is Player))
            {
                var component = ClientCache.FindAll<ComponentsRegistry>(GameObject.Lot).FirstOrDefault(c =>
                    c.Componenttype == (int) ComponentId.InventoryComponent);
                var itemTemplates = ClientCache.FindAll<Core.Client.InventoryComponent>(component.Componentid)
                    .Where(i => i.Itemid != default).ToArray();

                foreach (var itemTemplate in itemTemplates)
                {
                    if (itemTemplate.Itemid == default)
                        continue;
                    
                    var lot = (Lot) itemTemplate.Itemid;
                    var componentId = lot.GetComponentId(ComponentId.ItemComponent);
                    var itemComponent = await ClientCache.FindAsync<ItemComponent>(componentId);

                    if (itemComponent.ItemType == default)
                        continue;
                    
                    var item = await Item.Instantiate(GameObject, lot, default, (uint)(itemTemplate.Count ?? 1));
                    if (item != null)
                        Items[(EquipLocation) itemComponent.EquipLocation] = item;
                }
            }
        }

        /// <summary>
        /// For players the inventory has to be explicitly equipped due to component ordering
        /// </summary>
        public async Task EquipItemsAsync(IEnumerable<Item> items)
        {
            foreach (var item in items)
            {
                await EquipItemAsync(item);
            }
        }

        /// <summary>
        /// Updates a slot in the inventory, swapping a previous item for a new one
        /// </summary>
        /// <param name="item">The item to place in the slot</param>
        private async Task UpdateSlotAsync(Item item)
        {
            var equipLocation = item.ItemComponent.EquipLocation;
            if (Items.TryGetValue(equipLocation, out var previouslyEquippedItem))
            {
                await UnEquipItemAsync(previouslyEquippedItem);
            }

            Items[equipLocation] = item;
        }

        /// <summary>
        /// Gets the slot an object is potentially located at
        /// </summary>
        /// <param name="itemId">The object id to find the equiplocation for</param>
        /// <returns>The equip location if the object could be found, default otherwise</returns>
        private EquipLocation GetSlot(ObjectId itemId) => 
            Items.FirstOrDefault(i => i.Value == itemId) is {} keyValue 
                ? keyValue.Key
                : default;
        
        /// <summary>
        /// Whether the game object has this lot equipped
        /// </summary>
        /// <param name="lot">The lot to find items for</param>
        /// <returns><c>true</c> if the game object has this lot equipped</returns>
        public bool HasEquipped(Lot lot) => Items.Any(i => i.Value.Lot == lot);

        /// <summary>
        /// Equips an item for a game object, updating the equipped items list. Should be used at run-time, not at
        /// loading time as it also calls events and serializes the game object.
        /// </summary>
        /// <param name="item">The item to load for the player</param>
        /// <param name="ignoreAllChecks">Disables item checks, which checks for example if a player is building</param>
        public async Task EquipItemAsync(Item item, bool ignoreAllChecks = false)
        {
            var itemType = (ItemType) (item.ItemComponent.ItemType ?? (int) ItemType.Invalid);

            // TODO: What does this do?
            if (!ignoreAllChecks 
                && GameObject is Player player 
                && player.TryGetComponent<ModularBuilderComponent>(out var modularBuilderComponent)
                && !modularBuilderComponent.IsBuilding
                && (itemType == ItemType.Model || itemType == ItemType.LootModel || itemType == ItemType.Vehicle 
                    || item.Lot == 6086))
            {
                return;
            }

            await EnsureItemEquipped(item);
            foreach (var subItem in item.SubItems)
                await EquipItemAsync(subItem);
            
            GameObject.Serialize(GameObject);
        }

        /// <summary>
        /// Ensures that an item is equipped by setting it's equip state and mounting it in the skill component
        /// </summary>
        /// <param name="item">The item to equip</param>
        private async Task EnsureItemEquipped(Item item)
        {
            // If this item is already equipped, ignore
            if (Items.ContainsValue(item))
                return;
            
            await UpdateSlotAsync(item);
            item.IsEquipped = true;

            // Make sure that item sets are tracked for this item (if it has one)
            await ItemSet.CreateIfNewSet(this, item.Lot);
            await OnEquipped.InvokeAsync(item);
            
            // Update all the new skills the player gets from this item
            if (GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                await skillComponent.EquipItemAsync(item);
        }

        /// <summary>
        /// Unequips an item for a player.
        /// </summary>
        /// <remarks>Should be used at run-time instead of directly calling <see cref="UnEquipAsync"/> as this
        /// also calls unequip events</remarks>
        /// <param name="item">The item to unequip</param>
        public async Task UnEquipItemAsync(Item item)
        {
            if (item?.Id <= 0)
                return;
            
            if (item != null)
            {
                var rootItem = item.RootItem ?? item;
                await EnsureItemUnEquipped(rootItem);
            
                foreach (var subItem in rootItem.SubItems)
                {
                    if (Items.TryGetValue(GetSlot(subItem.Id), out var equippedSubItem))
                        await EnsureItemUnEquipped(equippedSubItem);
                }
            }

            GameObject.Serialize(GameObject);
        }

        /// <summary>
        /// Removes an item from the skill component and this inventory and unsets it's equip state 
        /// </summary>
        /// <param name="item">The item to unequip</param>
        private async Task EnsureItemUnEquipped(Item item)
        {
            if (GameObject.TryGetComponent<SkillComponent>(out var skillComponent))
                await skillComponent.UnequipItemAsync(item);
            
            await OnUnEquipped.InvokeAsync(item);
            
            item.IsEquipped = false;
            Items.Remove(GetSlot(item.Id));
        }

        /// <summary>
        /// Constructs the component
        /// </summary>
        /// <param name="writer">The writer to serialize to</param>
        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        /// <summary>
        /// Serializes the component
        /// </summary>
        /// <param name="writer">The writer to serialize to</param>
        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);

            var items = Items.Values.ToArray();
            writer.Write((uint) items.Length);

            foreach (var item in items)
            {
                writer.Write(item.Id);
                writer.Write(item.Lot);
                writer.WriteBit(false);
                writer.WriteBit(false);
                writer.WriteBit(false);
                writer.WriteBit(false);

                if (item.ItemComponent == default)
                {
                    writer.WriteBit(false);
                }
                else
                {
                    var info = item.Settings.ToString();
                    if (writer.Flag(!string.IsNullOrWhiteSpace(info)))
                    {
                        writer.WriteLdfCompressed(LegoDataDictionary.FromString(info));
                    }
                }
                writer.WriteBit(true);
            }
            writer.WriteBit(false);
        }
    }
}