using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using InfectedRose.Core;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class NiQuaternionProperty : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo Property { get; }

        /// <summary>
        /// Creates the packet property.
        /// </summary>
        /// <param name="property">Property to write.</param>
        public NiQuaternionProperty(PropertyInfo property)
        {
            // Store the property.
            this.Property = property;
        }
        
        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            // Write the property.
            var value = this.Property.GetValue(objectToWrite);
            writer.WriteNiQuaternion((Quaternion) value);

            // Store the written property.
            if (writtenProperties != null)
            {
                writtenProperties[this.Property.Name] = value;
            }
        }

        /// <summary>
        /// Reads the property to the reader.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to read.</param>
        /// <param name="reader">Bit reader to read to.</param>
        /// <param name="readProperties">Properties that were previously read.</param>
        /// <param name="context">Properties that provide context for reading, such as world zone ids.</param>
        public void Read(object objectToWrite, BitReader reader, Dictionary<string, object> readProperties, Dictionary<string, object> context)
        {
            // Set the value.
            object value = reader.ReadNiQuaternion();
            if (this.Property.CanWrite)
            {
                this.Property.SetValue(objectToWrite, value);
            }

            // Store the written property.
            if (readProperties != null)
            {
                readProperties[this.Property.Name] = this.Property.GetValue(objectToWrite);
            }
        }
    }
}