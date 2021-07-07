using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Uchu.Core.Client;

namespace Uchu.World.Client
{
    public abstract class BaseTableCache
    {
        /// <summary>
        /// Type of the table.
        /// </summary>
        public readonly Type TableType;

        /// <summary>
        /// Index of the table entries.
        /// </summary>
        public readonly PropertyInfo TableIndex;
        
        /// <summary>
        /// Client table property for the type.
        /// </summary>
        internal readonly PropertyInfo _cdClientContextProperty;

        /// <summary>
        /// Creates the base table cache.
        /// </summary>
        /// <param name="type">Type of the table.</param>
        /// <param name="index">Index of the table.</param>
        public BaseTableCache(Type type, PropertyInfo index)
        {
            this.TableType = type;
            this.TableIndex = index;
            
            var tableName = type.Name + "Table";
            foreach (var property in typeof(CdClientContext).GetProperties())
            {
                if (property.Name != tableName) continue;
                this._cdClientContextProperty = property;
                break;
            }
        }
        
        /// <summary>
        /// Converts an object to a key, such as for Lot ids.
        /// </summary>
        /// <param name="otherObject">Object to convert.</param>
        /// <returns>The converted object.</returns>
        internal static object ConvertKey(object otherObject)
        {
            if (otherObject is Lot lot)
            {
                return lot.Id;
            }
            return otherObject;
        }

        /// <summary>
        /// Returns all of the given index.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>All of the types that match the index.</returns>
        public virtual async Task<T[]> FindAllAsync<T>(object index) where T : class
        {
            throw new NotImplementedException("Not implemented in the given context.");
        }

        /// <summary>
        /// Returns the first value of the given index.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>The first value that matches the index.</returns>
        public virtual async Task<T> FindAsync<T>(object index) where T : class
        {
            return (await this.FindAllAsync<T>(index)).FirstOrDefault();
        }

        /// <summary>
        /// Loads the table into memory and indexes it with the
        /// stored index property.
        /// </summary>
        /// <returns>The indexed table.</returns>
        internal async Task<Dictionary<object, object[]>> IndexTableAsync()
        {
            // Read the table.
            await using var cdContext = new CdClientContext();
            var table = ((IQueryable<object>) this._cdClientContextProperty.GetValue(cdContext))?.ToArray();
            
            // Index and return the table.
            var indexedTable = new Dictionary<object, object[]>();
            foreach (var entry in table)
            {
                var index = this.TableIndex.GetValue(entry);
                if (indexedTable.ContainsKey(index))
                {
                    indexedTable[index] = indexedTable[index].Append(entry).ToArray();
                }
                else
                {
                    indexedTable.Add(index, new [] {entry});
                }
            }
            return indexedTable;
        }
    }
}