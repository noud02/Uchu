using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RakDotNet.IO;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    public class VendorComponent : ReplicaComponent
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

        public override void Construct(BitWriter writer)
        {
            Serialize(writer);
        }

        public override void Serialize(BitWriter writer)
        {
            writer.WriteBit(true);


            // This flag is only true if construction is true
            writer.WriteBit(true); // Is Active?
            writer.WriteBit(false); // Unknown Bit
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

            var vendorComponent = await ClientCache.FindAsync<Core.Client.VendorComponent>(componentId);

            var matrices = ClientCache.FindAll<Core.Client.LootMatrix>(vendorComponent.LootMatrixIndex);

            var shopItems = new List<ShopEntry>();

            foreach (var matrix in matrices)
            {
                shopItems.AddRange(ClientCache.FindAll<LootTable>(matrix.LootTableIndex)
                    .ToArray().Select(lootTable =>
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
            var itemComponent = await ClientCache.FindAsync<ItemComponent>(componentId);
            
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