using System.Collections.Generic;
using System.Reflection;
using RakDotNet.IO;

namespace Uchu.Core
{
    public abstract class WrappedPacketProperty : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo Property => WrappedProperty.Property;

        /// <summary>
        /// Packet property that is wrapped.
        /// </summary>
        public IPacketProperty WrappedProperty { get; }
        
        /// <summary>
        /// Creates the wrapped packet property.
        /// </summary>
        /// <param name="packetProperty">Packet property to wrap.</param>
        public WrappedPacketProperty(IPacketProperty packetProperty)
        {
            this.WrappedProperty = packetProperty;
        }
        
        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public abstract void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties);

        /// <summary>
        /// Reads the property to the reader.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to read.</param>
        /// <param name="reader">Bit reader to read to.</param>
        /// <param name="readProperties">Properties that were previously read.</param>
        /// <param name="context">Properties that provide context for reading, such as world zone ids.</param>
        public abstract void Read(object objectToWrite, BitReader reader, Dictionary<string, object> readProperties, Dictionary<string, object> context);
    }
}