using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Timer = System.Timers.Timer;

namespace Uchu.World.Client
{
    public class BurstCacheTable : BaseTableCache
    {
        /// <summary>
        /// Time before the cached data is cleared.
        /// </summary>
        private const int ClearTimeMilliseconds = 3000;
        
        /// <summary>
        /// Semaphore for reading the table. Used to ensure multiple threads/tasks
        /// aren't trying to create the cache at once.
        /// </summary>
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// 
        /// </summary>
        private Timer _resetTimer = new Timer(ClearTimeMilliseconds)
        {
            AutoReset = false,
        };
        
        /// <summary>
        /// Cache of the table.
        /// </summary>
        private Dictionary<object, object[]> _cachedTable;
        
        /// <summary>
        /// Creates the burst table cache.
        /// </summary>
        /// <param name="type">Type of the table.</param>
        /// <param name="index">Index of the table.</param>
        public BurstCacheTable(Type type, PropertyInfo index) : base(type, index)
        {
            // Set up the timer.
            this._resetTimer.Elapsed += (sender, args) =>
            {
                this._cachedTable = null;
                GC.Collect();
            };
        }
        
        /// <summary>
        /// Returns all of the given index.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>All of the types that match the index.</returns>
        public override async Task<T[]> FindAllAsync<T>(object index) where T : class
        {
            index = ConvertKey(index);
            
            // Load the table.
            await this._semaphore.WaitAsync();
            this._cachedTable ??= await this.IndexTableAsync();

            // Reset the timer and return the cached entry.
            this._semaphore.Release();
            this._resetTimer.Stop();
            this._resetTimer.Start();
            return (this._cachedTable.ContainsKey(index) ? this._cachedTable[index] : Array.Empty<object>()).Cast<T>().ToArray();
        }
    }
}