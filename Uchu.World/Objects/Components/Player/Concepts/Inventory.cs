using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;

namespace Uchu.World
{
    public class Inventory
    {
        private readonly List<Item> _items;
        
        public InventoryType InventoryType { get; }
        
        public InventoryManagerComponent ManagerComponent { get; }

        // TODO: Network & Store in DB
        public uint Size { get; set; } = 20;

        internal Inventory(InventoryType inventoryType, InventoryManagerComponent managerComponent)
        {
            InventoryType = inventoryType;
            ManagerComponent = managerComponent;

            using var ctx = new UchuContext();
            var playerCharacter = ctx.Characters
                .Include(c => c.Items)
                .First(c => c.CharacterId == managerComponent.GameObject.ObjectId);

            var inventoryItems = playerCharacter.Items
                .Where(item => (InventoryType) item.InventoryType == inventoryType)
                .ToList();

            _items = inventoryItems.Select(
                i => Item.Instantiate(i.InventoryItemId, this)
            ).Where(item => !ReferenceEquals(item, default)).ToList();

            foreach (var item in _items)
            {
                Object.Start(item);

                Logger.Information($"\t-> {item}");

                item.OnDestroyed.AddListener(() => { _items.Remove(item); });
            }
        }

        public IEnumerable<Item> Items => Array.AsReadOnly(_items.ToArray());

        public Item this[uint slot] => Items.FirstOrDefault(i => i.Slot == slot);

        public Item this[long id] => Items.FirstOrDefault(i => i.ObjectId == id);

        public void ManageItem(Item item)
        {
            _items.Add(item);
        }
        
        public void UnManageItem(Item item)
        {
            _items.Remove(item);
        }
    }
}