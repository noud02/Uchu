using System;

namespace Uchu.Core
{
    /// <summary>
    /// Indicates a bit flag is used for a property.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class Default : Attribute
    {
        /// <summary>
        /// Value that is checked. If the value matches, the
        /// flag will be set to false and the value will not
        /// be written/read.
        /// </summary>
        public object ValueToIgnore { get; }

        /// <summary>
        /// Creates the attribute with the value to ignore
        /// set to null for objects and default for structs.
        /// </summary>
        public Default()
        {
            this.ValueToIgnore = null;
        }
        
        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="valueToIgnore">Value to ignore writing.</param>
        public Default(object valueToIgnore)
        {
            this.ValueToIgnore = valueToIgnore;
        }
    }
}