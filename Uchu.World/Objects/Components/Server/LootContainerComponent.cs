using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core;
using Uchu.Core.Client;
using Uchu.World.Client;

namespace Uchu.World
{
    using Restriction = Func<Player, Lot, Task<bool>>;
    
    public class LootContainerComponent : Component
    {
        private LootContainerComponent()
        {
            Random = new Random();

            Entries = new List<LootMatrix>();

            Currencies = new List<CurrencyMatrix>();
            
            Restrictions = new List<Restriction>();
        }

        private Random Random { get; }

        private List<LootMatrix> Entries { get; }

        private List<CurrencyMatrix> Currencies { get; }
        
        private List<Restriction> Restrictions { get; }

        private int Level { get; set; }

        private int LootIndex { get; set; }

        private int CurrencyIndex { get; set; }

        private bool Searched { get; set; }

        public async Task CollectDetailsAsync()
        {
            if (Searched) return;

            await FindMatrixIndicesAsync();

            await QueryLootMatrixAsync();

            await QueryCurrencyMatrixAsync();

            Searched = true;
        }

        public void Restrict(Restriction restriction)
        {
            Restrictions.Add(restriction);
        }

        public async Task<Dictionary<Lot, uint>> GenerateLootYieldsAsync(Player owner)
        {
            var yields = new Dictionary<Lot, uint>();
            
            foreach (var matrix in Entries)
            {
                var count = Random.Next(matrix.Minimum, matrix.Maximum);
                var entries = matrix.Entries.ToList();
                
                if (owner.TryGetComponent<MissionInventoryComponent>(out var missionInventory))
                {
                    foreach (var entry in entries.Where(e => e.Mission).ToArray())
                    {
                        if (missionInventory.HasActiveForItem(entry.Lot))
                            continue;
                        entries.Remove(entry);
                    }
                }

                var filterList = entries.ToArray();
                foreach (var entry in filterList)
                {
                    var restricted = false;

                    foreach (var restriction in Restrictions)
                    {
                        if (await restriction(owner, entry.Lot)) continue;
                        
                        restricted = true;
                            
                        break;
                    }

                    if (restricted)
                    {
                        entries.Remove(entry);
                    }
                }
                
                for (var i = 0; i < count; i++)
                {
                    if (entries.Count == default) break;
                    
                    var entry = entries[Random.Next(default, entries.Count)];
                    
                    entries.Remove(entry);

                    var result = Random.NextDouble();

                    if (result <= matrix.Percentage)
                    {
                        yields.Add(entry.Lot, (uint)count);
                    }
                }
            }

            return yields;
        }

        public int GenerateCurrencyYields()
        {
            var yield = 0;

            foreach (var currency in Currencies)
            {
                if (currency.Level > Level) continue;

                var result = Random.Next(currency.Minimum, currency.Maximum);

                yield += result;
            }

            return yield;
        }

        private async Task QueryLootMatrixAsync()
        {
            var matrices = await ClientCache.FindAllAsync<Core.Client.LootMatrix>(LootIndex);

            foreach (var matrix in matrices)
            {
                var tables = await ClientCache.FindAllAsync<LootTable>(matrix.LootTableIndex);

                var items = tables.Select(t => new LootMatrixEntry
                {
                    Lot = t.Itemid ?? 0,
                    Priority = t.SortPriority ?? 0,
                    Mission = t.MissionDrop ?? false
                }).ToList();

                items.Sort((a, b) => a.Priority - b.Priority);

                var entry = new LootMatrix
                {
                    Minimum = matrix.MinToDrop ?? 1,
                    Maximum = matrix.MaxToDrop ?? 1,
                    Percentage = matrix.Percent ?? 0,
                    Entries = items.ToArray(),
                };

                Entries.Add(entry);
            }
        }

        private async Task QueryCurrencyMatrixAsync()
        {
            var matrices = await ClientCache.FindAllAsync<CurrencyTable>(CurrencyIndex);
            foreach (var matrix in matrices)
            {
                var entry = new CurrencyMatrix
                {
                    Level = matrix.Npcminlevel ?? 0,
                    Minimum = matrix.Minvalue ?? 0,
                    Maximum = matrix.Maxvalue ?? 0
                };

                Currencies.Add(entry);
            }
        }

        private async Task FindMatrixIndicesAsync()
        {
            var destructible = GameObject.Lot.GetComponentId(ComponentId.DestructibleComponent);

            if (destructible != default)
            {
                var component = (await ClientCache.FindAsync<Core.Client.DestructibleComponent>(destructible));
                LootIndex = component.LootMatrixIndex ?? 0;

                CurrencyIndex = component.CurrencyIndex ?? 0;

                Level = component.Level ?? 0;

                return;
            }

            var package = GameObject.Lot.GetComponentId(ComponentId.PackageComponent);

            if (package != default)
            {
                var component = await ClientCache.FindAsync<PackageComponent>(package);
                LootIndex = component.LootMatrixIndex ?? 0;
                CurrencyIndex = default;
                return;
            }

            LootIndex = default;

            CurrencyIndex = default;
        }
    }
}