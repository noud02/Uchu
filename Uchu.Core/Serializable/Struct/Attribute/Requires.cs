using System;

namespace Uchu.Core
{
    /// <summary>
    /// Indicates a property is only read from or written to
    /// if a property values matches.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class Requires : Attribute
    {
        /// <summary>
        /// Property name that is checked before attempting
        /// to read or write.
        /// </summary>
        public string PropertyName { get; set; }
        
        /// <summary>
        /// Value that is checked. If the value matches, the
        /// the property will be written/read.
        /// </summary>
        public object ValueToRequire { get; }
        
        /// <summary>
        /// Creates the attribute.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="valueToRequire">Value required.</param>
        public Requires(string propertyName, object valueToRequire)
        {
            this.PropertyName = propertyName;
            this.ValueToRequire = valueToRequire;
        }
        
        /// <summary>
        /// Creates the attribute.
        /// The Value To Require will be the bool true.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        public Requires(string propertyName) : this(propertyName, true)
        {
            
        }
    }
}