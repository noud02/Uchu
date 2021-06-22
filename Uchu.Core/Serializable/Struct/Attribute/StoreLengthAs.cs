using System;

namespace Uchu.Core
{
    /// <summary>
    /// Overrides the type used for the length of an array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class StoreLengthAs : Attribute
    {
        /// <summary>
        /// Type that is used for the array length. 
        /// </summary>
        public Type Type;
        
        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="type">Type that is used for the array length.</param>
        public StoreLengthAs(Type type)
        {
            this.Type = type;
        }
    }
}