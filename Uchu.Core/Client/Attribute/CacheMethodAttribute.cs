using System;

namespace Uchu.Core.Client.Attribute
{
    public enum CacheMethod
    {
        /// <summary>
        /// Stores individual entries for a given id in the cache.
        /// </summary>
        Entry,
        
        /// <summary>
        /// Temporarily stores the entire table in memory until
        /// it isn't used.
        /// </summary>
        Burst,
        
        /// <summary>
        /// Permanently stores the entire table in memory. Intended
        /// only for tables that are slow to load and have a
        /// noticeable impact to loading, such as skills.
        /// </summary>
        Persistent,
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class CacheMethodAttribute : System.Attribute
    {
        /// <summary>
        /// Cache method to use for the table.
        /// </summary>
        public readonly CacheMethod Method;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        public CacheMethodAttribute(CacheMethod method)
        {
            this.Method = method;
        }
    }
}