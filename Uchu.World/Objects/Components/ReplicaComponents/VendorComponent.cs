using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class VendorComponent : StructReplicaComponent<VendorSerialization,VendorSerialization>
    {
        public override ComponentId Id => ComponentId.VendorComponent;
        
        public ShopEntry[] Entries { get; set; }
        
        public Event<Lot, uint, Player> OnBuy { get; }
        
        public Event<Item, uint, Player> OnSell { get; }
        
        public Event<Item, uint, Player> OnBuyback { get; }

        protected VendorComponent()
        {
            OnBuy = new Event<Lot, uint, Player>();
            
            OnSell = new Event<Item, uint, Player>();
            
            OnBuyback = new Event<Item, uint, Player>();

            Listen(OnStart, async () =>
            {
                await SetupEntries();
                
                Listen(GameObject.OnInteract, OnInteract);
            });
        }
        
        /// <summary>
        /// Creates the Construct packet for the replica component.
        /// </summary>
        /// <returns>The Construct packet for the replica component.</returns>
        public override VendorSerialization GetConstructPacket()
        {
            var packet = base.GetConstructPacket();
            packet.UnknownFlag1 = true;
            packet.UnknownFlag2 = true;
            packet.UnknownFlag3 = false;
            return packet;
        }
        
        /// <summary>
        /// Creates the Serialize packet for the replica component.
        /// </summary>
        /// <returns>The Serialize packet for the replica component.</returns>
        public override VendorSerialization GetSerializePacket()
        {
            return this.GetConstructPacket();
        }

        private void OnInteract(Player player)
        {
            player.Message(new OpenVendorWindowMessage
            {
                Associate = GameObject
            });
            
            player.Message(new VendorStatusUpdateMessage
            {
                Associate = GameObject,
                Entries = Entries
            });
        }

        private async Task SetupEntries()
        {
            var componentId = GameObject.Lot.GetComponentId(ComponentId.VendorComponent);

            var vendorComponent = (await ClientCache.GetTableAsync<Core.Client.VendorComponent>()).First(c => c.Id == componentId);

            var matrices = ClientCache.GetTable<Core.Client.LootMatrix>().Where(l => l.LootMatrixIndex == vendorComponent.LootMatrixIndex);

            var shopItems = new List<ShopEntry>();

            foreach (var matrix in matrices)
            {
                shopItems.AddRange(ClientCache.GetTable<LootTable>().Where(
                    l => l.LootTableIndex == matrix.LootTableIndex
                ).ToArray().Select(lootTable =>
                {
                    Debug.Assert(lootTable.Itemid != null, "lootTable.Itemid != null");
                    Debug.Assert(lootTable.SortPriority != null, "lootTable.SortPriority != null");
                    
                    return new ShopEntry
                    {
                        Lot = new Lot(lootTable.Itemid.Value),
                        SortPriority = lootTable.SortPriority.Value
                    };
                }));
            }

            Entries = shopItems.ToArray();
        }

        public async Task Buy(Lot lot, uint count, Player player)
        {
            var componentId = lot.GetComponentId(ComponentId.ItemComponent);
            var itemComponent = (await ClientCache.GetTableAsync<ItemComponent>()).First(
                i => i.Id == componentId
            );
            
            if (count == default || itemComponent.BaseValue <= 0) return;

            var character = player.GetComponent<CharacterComponent>();
            var cost = (uint) ((itemComponent.BaseValue ?? 0) * count);

            if (cost > character.Currency)
                return;
            
            // If we have to buy this with an alternative currency
            if (itemComponent.CurrencyLOT != null)
            {
                var alternativeCurrencyLot = (Lot) itemComponent.CurrencyLOT;
                var alternativeCurrencyCost = itemComponent.AltCurrencyCost ?? 0;
                var inventory = player.GetComponent<InventoryManagerComponent>();
                
                if (alternativeCurrencyCost > inventory.FindItems(alternativeCurrencyLot)
                    .Select(i => (int) i.Count).Sum())
                    return;

                await inventory.RemoveLotAsync(alternativeCurrencyLot, (uint) alternativeCurrencyCost);
            }
            
            character.Currency -= cost;
            
            await player.GetComponent<InventoryManagerComponent>().AddLotAsync(lot, count, lootType: LootType.Vendor);
            
            player.Message(new VendorTransactionResultMessage
            {
                Associate = GameObject,
                Result = TransactionResult.Success
            });

            await OnBuy.InvokeAsync(lot, count, player);
        }

        public async Task Sell(Item item, uint count, Player player)
        {
            if (count == default || item.ItemComponent.BaseValue <= 0)
                return;
            
            var inventory = player.GetComponent<InventoryManagerComponent>();
            var character = player.GetComponent<CharacterComponent>();
            
            await inventory.MoveItemBetweenInventoriesAsync(
                item,
                count,
                item.Inventory.InventoryType,
                InventoryType.VendorBuyback
            );
            
            var sellMultiplier = item.ItemComponent.SellMultiplier ?? 0.1f;
            var returnCurrency = Math.Floor((item.ItemComponent.BaseValue ?? 0) * sellMultiplier) * count;

            character.Currency += (uint) returnCurrency;

            // If this was bought with alt currency, give that back too
            if (item.ItemComponent.CurrencyLOT != null)
            {
                var alternativeCurrencyLot = (Lot) item.ItemComponent.CurrencyLOT;
                var returnAlternativeCurrency = (uint) Math.Floor((item.ItemComponent.AltCurrencyCost ?? 0) * sellMultiplier) * count;
                await inventory.AddLotAsync(alternativeCurrencyLot, returnAlternativeCurrency, lootType: LootType.Vendor);
            }
            
            player.Message(new VendorTransactionResultMessage
            {
                Associate = GameObject,
                Result = TransactionResult.Success
            });

            await OnSell.InvokeAsync(item, count, player);
        }

        public async Task Buyback(Item item, uint count, Player player)
        {
            var itemComponent = item.ItemComponent;
            if (count == default || itemComponent.BaseValue <= 0)
                return;

            var character = player.GetComponent<CharacterComponent>();
            var inventory = player.GetComponent<InventoryManagerComponent>();
            
            var sellMultiplier = itemComponent.SellMultiplier ?? 0.1f;
            var cost = (uint) Math.Floor((itemComponent.BaseValue ?? 0) * sellMultiplier) * count;
            if (cost > character.Currency)
                return;
            
            // If we have to buy this with an alternative currency
            if (itemComponent.CurrencyLOT != null)
            {
                var alternativeCurrencyLot = (Lot) itemComponent.CurrencyLOT;
                var alternativeCurrencyCost = (uint) Math.Floor((item.ItemComponent.AltCurrencyCost ?? 0) * sellMultiplier) * count;
                
                if (alternativeCurrencyCost > inventory.FindItems(alternativeCurrencyLot)
                    .Select(i => (int) i.Count).Sum())
                    return;

                await inventory.RemoveLotAsync(alternativeCurrencyLot, alternativeCurrencyCost);
            }

            character.Currency -= cost;
            
            await inventory.RemoveItemAsync(item, count, InventoryType.VendorBuyback);
            await inventory.AddLotAsync(item.Lot, count, lootType: LootType.Vendor);
            
            player.Message(new VendorTransactionResultMessage
            {
                Associate = GameObject,
                Result = TransactionResult.Success
            });

            await OnBuyback.InvokeAsync(item, count, player);
        }
    }
}