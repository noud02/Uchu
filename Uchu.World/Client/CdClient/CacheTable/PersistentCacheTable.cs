using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Uchu.World.Client
{
    public class PersistentCacheTable : BaseTableCache
    {
        /// <summary>
        /// Cache of the table.
        /// </summary>
        private Dictionary<object, object[]> _cachedTable;
        
        /// <summary>
        /// Creates the persistent table cache.
        /// </summary>
        /// <param name="type">Type of the table.</param>
        /// <param name="index">Index of the table.</param>
        public PersistentCacheTable(Type type, PropertyInfo index) : base(type, index)
        {
            
        }
        
        /// <summary>
        /// Loads the table.
        /// Called after the table is created.
        /// </summary>
        public async Task LoadAsync()
        {
            this._cachedTable = await this.IndexTableAsync();
        }
        
        /// <summary>
        /// Returns all of the given index.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>All of the types that match the index.</returns>
        public override async Task<T[]> FindAllAsync<T>(object index) where T : class
        {
            index = ConvertKey(index);
            return (this._cachedTable.ContainsKey(index) ? this._cachedTable[index] : Array.Empty<object>()).Cast<T>().ToArray();
        }
    }
}