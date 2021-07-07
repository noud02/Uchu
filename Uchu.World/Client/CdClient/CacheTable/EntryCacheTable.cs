using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Uchu.Core.Client;

namespace Uchu.World.Client
{
    public class EntryCacheTable : BaseTableCache
    {
        /// <summary>
        /// Cache of the table.
        /// </summary>
        private Dictionary<object, object[]> _cachedTable = new Dictionary<object, object[]>();
        
        /// <summary>
        /// Semaphore for reading the table. Used to ensure multiple threads/tasks
        /// aren't trying to create the cache at once.
        /// </summary>
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        /// <summary>
        /// Name of the column to read.
        /// </summary>
        private string _columnName;
        
        /// <summary>
        /// Common context used for the cache tables.
        /// </summary>
        private static CdClientContext ClientContext = new CdClientContext();
        
        /// <summary>
        /// Creates the entry table cache.
        /// </summary>
        /// <param name="type">Type of the table.</param>
        /// <param name="index">Index of the table.</param>
        public EntryCacheTable(Type type, PropertyInfo index) : base(type, index)
        {
            this._columnName = this.TableIndex.GetCustomAttribute<ColumnAttribute>()?.Name;
        }
        
        /// <summary>
        /// Returns all of the given index.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>All of the types that match the index.</returns>
        public override async Task<T[]> FindAllAsync<T>(object index) where T : class
        {
            index = ConvertKey(index);
            
            // Populate the cache entry.
            await this._semaphore.WaitAsync();
            if (!this._cachedTable.ContainsKey(index))
            {
                // Due to Reflection, LINQ isn't suitable in this case.
                var table = (DbSet<T>) this._cdClientContextProperty.GetValue(ClientContext);
                var sqlCommand = $"SELECT * FROM {this.TableType.Name} WHERE {this._columnName} = {index}";
                this._cachedTable[index] = table?.FromSqlRaw(sqlCommand).ToArray<object>();
            }

            // Return the cached entry.
            this._semaphore.Release();
            return (this._cachedTable.ContainsKey(index) ? this._cachedTable[index] : Array.Empty<object>()).Cast<T>().ToArray();
        }
    }
}