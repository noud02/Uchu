using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;
using InfectedRose.Lvl;
using IronPython.Modules;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.Core.Resources;
using Uchu.World.Client;
using Uchu.World.Objects.Components;

namespace Uchu.World
{
    /// <summary>
    /// Component that handles the actual inventories of a game object, not to be confused with <see cref="InventoryComponent"/>
    /// which represents the equipped inventory of a game object.
    /// </summary>
    public class InventoryManagerComponent : Component, ISavableComponent
    {
        #region properties
        
        /// <summary>
        /// Contains all the inventories this game object has, indexed by inventory type
        /// </summary>
        private readonly Dictionary<InventoryType, Inventory> _inventories = new Dictionary<InventoryType, Inventory>();
        
        /// <summary>
        /// Event called when a lot is added
        /// </summary>
        public Event<Lot, uint> OnLotAdded { get; } = new Event<Lot, uint>();

        /// <summary>
        /// Event called when a lot is removed
        /// </summary>
        public Event<Lot, uint> OnLotRemoved { get; } = new Event<Lot, uint>();

        /// <summary>
        /// Dictionary of lots and locks that allows to lock a single lot in the inventory
        /// </summary>
        /// <remarks>
        /// Use <see cref="GetLotLock"/> to interact with this dictionary to ensure proper updates of keys.
        /// </remarks>
        private Dictionary<int, SemaphoreSlim> LotLock { get; } = new Dictionary<int, SemaphoreSlim>();

        /// <summary>
        /// All inventories in this inventory manager
        /// </summary>
        public Inventory[] Inventories => _inventories.Values.ToArray();

        /// <summary>
        /// All the items located in all inventories
        /// </summary>
        public Item[] Items => _inventories.Values.SelectMany(i => i.Items).ToArray();
        #endregion properties

        #region constructors
        protected InventoryManagerComponent()
        {
            Listen(OnStart, async () =>
            {
                await using var uchuContext = new UchuContext();
                
                var playerCharacter = uchuContext.Characters
                    .Include(c => c.Items)
                    .First(c => c.Id == GameObject.Id);

                var inventoryTypesToCreate = playerCharacter.Items
                    .Select(i => (InventoryType) i.InventoryType)
                    .Append(InventoryType.Items)       //
                    .Append(InventoryType.VaultItems)  // These 3 need to be available to be able to increase their Size
                    .Append(InventoryType.VaultModels) //
                    .Distinct();
                
                // Load all the already owned items of the player
                foreach (var inventoryType in inventoryTypesToCreate)
                {
                    
                    Logger.Debug($"Loading {inventoryType} inventory.");
                    var inventory = new Inventory(inventoryType, this);
                    
                    await inventory.LoadItems(
                        playerCharacter.Items
                            .Where(item => 
                                item.ParentId == ObjectId.Invalid
                                && (InventoryType) item.InventoryType == inventoryType)
                        );
                    
                    _inventories.Add(inventoryType, inventory);
                }

                // Finally load all the sub items
                if (this[InventoryType.Hidden] is {} hiddenInventory)
                    await hiddenInventory.LoadItems(playerCharacter.Items.Where(i => i.ParentId != ObjectId.Invalid 
                        && (InventoryType) i.InventoryType == InventoryType.Hidden));
            });

            Listen(OnDestroyed, () =>
            {
                OnLotAdded.Clear();
                OnLotRemoved.Clear();
                
                foreach (var item in _inventories.Values.SelectMany(inventory => inventory.Items))
                    Destroy(item);
            });
        }
        #endregion constructors
        
        #region saving

        public async Task SaveAsync(UchuContext context)
        {
            var itemsToSave = Items;

            var character = await context.Characters.Where(c => c.Id == GameObject.Id)
                .Include(c => c.Items)
                .FirstAsync();

            // Delete old items
            var savedItemsToDelete = character.Items.Where(savedItem => itemsToSave
                .All(itemToSave => itemToSave.Id != savedItem.Id)).ToArray();
            foreach (var savedItemToDelete in savedItemsToDelete)
            {
                character.Items.Remove(savedItemToDelete);
            }
            
            // Update existing items and add new items
            foreach (var itemToSave in itemsToSave)
            {
                var savedItem = character.Items.FirstOrDefault(i => i.Id == itemToSave.Id); 
                if (savedItem == default)
                {
                    character.Items.Add(new InventoryItem
                    {
                        Id = itemToSave.Id,
                        Lot = itemToSave.Lot,
                        Slot = (int) itemToSave.Slot,
                        Count = itemToSave.Count,
                        IsBound = itemToSave.IsBound,
                        IsEquipped = itemToSave.IsEquipped,
                        InventoryType = (int) (itemToSave.Inventory?.InventoryType ?? InventoryType.None),
                        ExtraInfo = itemToSave.Settings.ToString(),
                        ParentId = itemToSave.RootItem?.Id ?? ObjectId.Invalid
                    });
                }
                else
                {
                    savedItem.Slot = (int) itemToSave.Slot;
                    savedItem.Count = itemToSave.Count;
                    savedItem.IsBound = itemToSave.IsBound;
                    savedItem.IsEquipped = itemToSave.IsEquipped;
                    savedItem.InventoryType = (int) (itemToSave.Inventory?.InventoryType ?? InventoryType.None);
                    savedItem.ExtraInfo = itemToSave.Settings.ToString();
                    savedItem.ParentId = itemToSave.RootItem?.Id ?? ObjectId.Invalid;
                }
            }

            Logger.Debug($"Saved inventory for {GameObject}");
        }

        #endregion saving
        
        #region operators
        
        public Inventory this[InventoryType inventoryType] => _inventories.TryGetValue(inventoryType, out var inventory) 
            ? inventory : default;

        #endregion operators

        #region methods
        
        #region finditem

        /// <summary>
        /// Finds and returns the first item of a certain lot throughout all inventories
        /// </summary>
        /// <param name="lot">The lot of the item to look for</param>
        /// <returns>The item retrieved from the entire inventory</returns>
        public Item FindItem(Lot lot)
        {
            return _inventories.Values.Select(
                inventory => inventory.Items.FirstOrDefault(i => i.Lot == lot)
            ).FirstOrDefault(item => item != default);
        }

        /// <summary>
        /// Finds and returns the first item of a certain lot in a certain inventory
        /// </summary>
        /// <param name="lot">The lot of the item to look for</param>
        /// <param name="inventoryType">The inventory type to look in for the item</param>
        /// <returns>The item retrieved from the given inventory</returns>
        public Item FindItem(Lot lot, InventoryType inventoryType)
        {
            return _inventories[inventoryType].Items.FirstOrDefault(i => i.Lot == lot);
        }

        /// <summary>
        /// Finds and returns the first item of a certain lot in a certain inventory given that the provided argument
        /// item is its parent
        /// </summary>
        /// <param name="lot">The lot of the item to look for</param>
        /// <param name="inventoryType">The inventory type to look in for the item</param>
        /// <param name="rootItem">The root item that this item needs to belong to</param>
        /// <returns>The item retrieved from the given inventory</returns>
        public Item FindItem(Lot lot, InventoryType inventoryType, Item rootItem)
        {
            return _inventories[inventoryType].Items.FirstOrDefault(i => i.Lot == lot && i.RootItem?.Id == rootItem.Id);
        }

        /// <summary>
        /// Finds an item by looking for the provided lot in the given inventory type and ensuring that said item has a
        /// count of at least minimumCount
        /// </summary>
        /// <param name="lot">The lot to use</param>
        /// <param name="inventoryType">The inventory to search in</param>
        /// <param name="minimumCount">The minimum count that the item should have</param>
        /// <returns>The item if found, else default</returns>
        public Item FindItem(Lot lot, InventoryType inventoryType, uint minimumCount)
        {
            return FindItems(lot, inventoryType).FirstOrDefault(item => item.Count >= minimumCount);
        }

        /// <summary>
        /// Finds all the items of a certain lot, in all inventories
        /// </summary>
        /// <param name="lot">The lot to look for</param>
        /// <returns>List of all found items</returns>
        public Item[] FindItems(Lot lot) => _inventories.Values.SelectMany(
            inventory => inventory.Items.Where(i => i.Lot == lot)
            ).ToArray();

        /// <summary>
        /// Finds all the items of a certain lot from a certain inventory
        /// </summary>
        /// <param name="lot">The lot to find items for</param>
        /// <param name="inventoryType">The inventory type to look in</param>
        /// <returns>List of all found items</returns>
        public Item[] FindItems(Lot lot, InventoryType inventoryType) =>
            _inventories[inventoryType].Items.Where(i => i.Lot == lot).ToArray();

        #endregion finditem
        
        /// <summary>
        /// Returns the lock for a lot, allowing locking a single lot in the inventory
        /// </summary>
        /// <param name="lot">The lot to acquire the lock for</param>
        /// <returns>The lock for the lot</returns>
        private SemaphoreSlim GetLotLock(Lot lot)
        {
            if (!LotLock.ContainsKey(lot.Id))
                LotLock[lot.Id] = new SemaphoreSlim(1, 1);
            return LotLock[lot.Id];
        }

        /// <summary>
        /// Returns the total sum of items a game object has of a certain lot.
        /// </summary>
        /// <param name="lot">The lot to calculate the count for</param>
        /// <returns>The total count of a certain lot in the inventory</returns>
        private uint InventoryCount(Lot lot)
        {
            return (uint) Items.Where(i => i.Lot == lot).Sum(i => i.Count);
        }

        /// <summary>
        /// Checks if a lot is a faction token proxy and tries to assign the proper faction token according to a
        /// possibly related character component. In case there's no character component for the GameObject or the
        /// lot is isn't a faction token proxy, nothing is done.
        /// </summary>
        /// <param name="lot">The lot to check</param>
        /// <returns>Whether the lot is valid for use, in case of a faction token proxy this means that the proxy
        /// could be properly replaced with a faction token</returns>
        private bool HandleFactionToken(ref Lot lot)
        {
            // If this is not a faction token or the game object doesn't have a character, there's nothing to check
            if (lot != Lot.FactionTokenProxy)
                return true;
            if (!GameObject.TryGetComponent<CharacterComponent>(out var character))
                return true;

            var token = character.FactionToken;
            if (token == Lot.FactionTokenProxy)
                return false;
            lot = token;
            
            return true;
        }

        /// <summary>
        /// Returns the item component for a lot, default if either the item registry or item component doesn't exist for
        /// the lot
        /// </summary>
        /// <param name="lot">The lot to find</param>
        /// <returns>The item component for the lot, if available</returns>
        private static async Task<ItemComponent> GetItemComponentForLotAysnc(Lot lot)
        {
            var itemRegistryEntry = (await ClientCache.GetTableAsync<ComponentsRegistry>()).FirstOrDefault(
                r => r.Id == lot && r.Componenttype == (int)ComponentId.ItemComponent);
            if (itemRegistryEntry == default)
                return default;

            var itemComponent = (await ClientCache.GetTableAsync<ItemComponent>()).FirstOrDefault(
                i => i.Id == itemRegistryEntry.Componentid);

            return itemComponent;
        }

        /// <summary>
        /// Adds a new lot to the inventory. The lot will automatically instantiated as the required amount of items
        /// based on the provided count and the stack size of that item.
        /// </summary>
        /// <remarks>
        /// The returning list may be empty if it was possible to distribute over already existing stacks
        /// </remarks>
        /// <param name="lot">The lot to add to the inventory</param>
        /// <param name="count">The count of the lot we want to add to the inventory</param>
        /// <param name="settings">Optional <c>LegoDataDictionary</c> to instantiate the item with</param>
        /// <param name="inventoryType">Optional explicit inventory type to add this lot to, if not provided this will be
        ///     implicitly retrieved from the item lot, generally used for vendor buy back</param>
        /// <param name="rootItem">An optional parent item</param>
        /// <param name="lootType">The reason this lot is given to the player</param>
        /// <param name="showFlyingLoot">Whether to show flying loot animation</param>
        /// <returns>The list of items that were created while adding the lot</returns>
        public async Task AddLotAsync(Lot lot, uint count, LegoDataDictionary settings = default,
            InventoryType inventoryType = default, Item rootItem = default, LootType lootType = default,
            bool showFlyingLoot = true)
        {
            var createdItems = new List<Item>();
            
            // For players, the faction token proxy needs to be replaced with an actual faction token
            // If this wasn't possible, exit
            if (!HandleFactionToken(ref lot))
                return;

            var itemComponent = await GetItemComponentForLotAysnc(lot);
            if (itemComponent == default)
                return;

            inventoryType = inventoryType != default 
                ? inventoryType 
                : ((ItemType) itemComponent.ItemType).GetInventoryType();
            
            // Get the proper inventory for this game object, if it has no inventory of that type yet, create it
            if (!_inventories.TryGetValue(inventoryType, out var inventory))
            {
                inventory = new Inventory(inventoryType, this);
                _inventories[inventoryType] = inventory;
            }

            // Some items have no stack size, like bricks
            var stackSize = itemComponent.StackSize ?? 1;
            if (stackSize == default) 
                stackSize = int.MaxValue;
            
            var totalAdded = 0L;
            var totalToAdd = count;

            // Acquire the lock for a single lot to update the lot in the inventory
            var lotLock = GetLotLock(lot);
            await lotLock.WaitAsync();

            try
            {
                // Fill all old stacks
                foreach (var item in inventory.Items.Where(i => i.Lot == lot && i.Count < stackSize))
                {
                    var toAdd = (uint) Min(stackSize, (int) totalToAdd, (int) (stackSize - item.Count));

                    await item.IncrementCountAsync(toAdd);
                    
                    totalToAdd -= toAdd;
                    totalAdded += toAdd;

                    // Exit if no new stacks are required to hold the lot
                    if (totalToAdd <= 0)
                        return;
                }
                
                // Create new stacks
                while (totalToAdd > 0)
                {
                    var toAdd = (uint) Min(stackSize, (int) totalToAdd);
                    var item = await Item.Instantiate(GameObject, lot, inventory, toAdd, extraInfo: settings, rootItem: rootItem);

                    totalToAdd -= toAdd;
                    totalAdded += toAdd;

                    // Item was successfully instantiated
                    if (item != null)
                    {
                        Start(item);
                        item.MessageCreation(showFlyingLoot);
                        createdItems.Add(item);
                    }
                    // Might occur if the inventory is full or an error occured during slot claiming
                    else
                    {
                        // Display item-bouncing-off-backpack animation
                        ((Player) GameObject).Message(new NotifyRewardMailedMessage()
                        {
                            ObjectId = ObjectId.Standalone,
                            StartPoint = new Vector3(0, 0, 0),
                            TemplateId = lot,
                            Associate = GameObject,
                        });

                        await using var uchuContext = new UchuContext();

                        var playerCharacter = uchuContext.Characters
                            .First(c => c.Id == GameObject.Id);

                        var mail = new CharacterMail
                        {
                            RecipientId = playerCharacter.Id,
                            AuthorId = 0,
                            AttachmentLot = lot,
                            AttachmentCount = (ushort) toAdd,
                            SentTime = DateTime.Now,
                            ExpirationTime = DateTime.Now
                        };

                        switch (lootType)
                        {
                            // Achievements
                            case LootType.Achievement:
                            {
                                mail.Subject = "%[MAIL_ACHIEVEMENT_OVERFLOW_HEADER]";
                                mail.Body = "%[MAIL_ACHIEVEMENT_OVERFLOW_BODY]";
                                break;
                            }
                            // Activities - not entirely sure when this will be used
                            // The text also works well enough for missions, though those should not add items to full inventories anyway
                            case LootType.Mission:
                            case LootType.Activity:
                            {
                                mail.Subject = "%[MAIL_ACTIVITY_OVERFLOW_HEADER]";
                                mail.Body = "%[MAIL_ACTIVITY_OVERFLOW_BODY]";
                                break;
                            }
                            // Sometimes happens when picking up items.
                            case LootType.Pickup:
                            {
                                // To avoid spamming people's mailboxes, don't send the item if the player has 20+ mails.
                                var mailCount = uchuContext.Mails.Count(m => m.RecipientId == playerCharacter.Id);
                                if (mailCount >= 20)
                                    continue;

                                mail.Subject = "Lost Item";
                                mail.Body = "You picked up this item, but didn't have room for it in your backpack.";
                                break;
                            }
                            // /gmadditem, item sets, and for any other reason listed in the LootType enum
                            default:
                            {
                                mail.Subject = "Lost Item";
                                mail.Body = "You received this item, but didn't have room for it in your backpack.";
                                break;
                            }
                        }

                        await uchuContext.Mails.AddAsync(mail);
                        await uchuContext.SaveChangesAsync();
                    }
                }
            }
            finally
            {
                lotLock.Release();
                
                var _ = Task.Run(async () =>
                {
                    // Complete all the collectible tasks for this game object
                    if (GameObject.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                    {
                        for (var i = 0; i < totalAdded; i++)
                        {
                            await missionInventory.ObtainItemAsync(lot);
                        }
                    }
                
                    await OnLotAdded.InvokeAsync(lot, (uint)totalAdded);
                });
            }
            
            // Create sub items if this was a root item
            if (rootItem == null)
                foreach (var createdItem in createdItems)
                foreach (var subItemLot in createdItem.SubItemLots)
                    await AddLotAsync(subItemLot, 1, default, InventoryType.Hidden, createdItem);
        }

        /// <summary>
        /// Removes all occurrences of a certain lot from the inventory
        /// </summary>
        /// <param name="lot">The lot to remove</param>
        /// <param name="silent">Whether to notify the game object of the updated item counts</param>
        public Task RemoveAllAsync(Lot lot, bool silent = false)
        {
            return RemoveLotAsync(lot, InventoryCount(lot), silent: silent);
        }

        /// <summary>
        /// Removes an item from a specific slot in the inventory, for example useful if the player wanted to delete
        /// a specific item
        /// </summary>
        /// <param name="item">The item to remove</param>
        /// <param name="count">The number of items to remove, if left empty will use the count from the item</param>
        /// <param name="inventoryType">The inventory to remove the item from, if left empty will use the one from the item inventory</param>
        /// <param name="silent">Whether or not to notify the client</param>
        public async Task RemoveItemAsync(Item item, uint count = default, InventoryType inventoryType = default,
            bool silent = false)
        {
            var lotLock = GetLotLock(item.Lot);
            await lotLock.WaitAsync();

            try
            {
                if (inventoryType == default)
                    inventoryType = item.Inventory.InventoryType;
                if (count == default)
                    count = item.Count;

                var itemToRemove = _inventories[inventoryType][item.Slot];
                await itemToRemove.DecrementCountAsync(count, silent);
                await OnLotRemoved.InvokeAsync(item.Lot, count);
            }
            finally
            {
                lotLock.Release();
            }
        }

        /// <summary>
        /// Removes <c>count</c> items of a certain lot from inventory of the GameObject. This will look for the items
        /// to delete itself, if you wish to delete a specific item use <see cref="RemoveItemAsync"/>
        /// </summary>
        /// <param name="lot">The lot of the item to remove</param>
        /// <param name="count">The number of items to remove</param>
        /// <param name="inventoryType">Optional explicit inventory type to remove items from, generally used for
        ///     vendor buyback</param>
        /// <param name="silent">Whether to send the updated count to the game object or not</param>
        /// <exception cref="InvalidOperationException">If the passed item does not have an item component</exception>
        public async Task RemoveLotAsync(Lot lot, uint count, InventoryType inventoryType = default,
            bool silent = false)
        {
            var lotLock = GetLotLock(lot);
            await lotLock.WaitAsync();

            try
            {
                // Sort to make sure we remove from the stacks with the lowest count first.
                var itemsToRemove = (inventoryType == default ? FindItems(lot) : FindItems(lot, inventoryType)).ToList();
                itemsToRemove.Sort((i1, i2) => (int) (i1.Count - i2.Count));

                foreach (var itemToRemove in itemsToRemove)
                {
                    var amountToRemove = (uint) Min((int) count, (int) itemToRemove.Count);
                    await itemToRemove.DecrementCountAsync(amountToRemove, silent);
   
                    count -= amountToRemove;
                    if (count == 0)
                        return;
                }
            }
            finally
            {
                await OnLotRemoved.InvokeAsync(lot, count);
                lotLock.Release();
            }
        }

        /// <summary>
        /// Searches for the first occurance of the lot in the source inventory and moves that item to the destination
        /// inventory.
        /// </summary>
        /// <param name="lot">The lot to find and move</param>
        /// <param name="count">The number of items to move</param>
        /// <param name="source">The source inventory</param>
        /// <param name="destination">The destination inventory</param>
        /// <param name="silent">Whether to notify the client or not</param>
        public async Task MoveLotBetweenInventoriesAsync(Lot lot, uint count, InventoryType source,
            InventoryType destination,
            bool silent = false)
        {
            var itemToMove = FindItem(lot, source, count);
            await MoveItemBetweenInventoriesAsync(itemToMove, count, source, destination, silent);
        }

        /// <summary>
        /// Moves an item from one inventory to another inventory, generally used for vendor buy back
        /// </summary>
        /// <param name="item">The item to move</param>
        /// <param name="count">The amount of items to move</param>
        /// <param name="source">The source inventory to move from</param>
        /// <param name="destination">The destination inventory to move to</param>
        /// <param name="silent">Whether to send inventory update messages or not</param>
        /// <param name="showFlyingLoot">Whether to show flying loot animation</param>
        public async Task MoveItemBetweenInventoriesAsync(Item item, uint count, InventoryType source,
            InventoryType destination, bool silent = false, bool showFlyingLoot = false)
        {
            if (item == null)
                return;

            await RemoveItemAsync(item, count, source, silent);
            await AddLotAsync(item.Lot , count, item.Settings, destination, lootType: LootType.Inventory, showFlyingLoot: showFlyingLoot); // TODO: find out if lootType is correct (what's LootType.Relocate?)
        }
        
        #endregion methods
        
        private static int Min(params int[] nums)
        {
            return nums.Min();
        }
    }
}