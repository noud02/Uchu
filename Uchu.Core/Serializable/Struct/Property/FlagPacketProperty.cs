using System;
using System.Collections.Generic;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class FlagPacketProperty : WrappedPacketProperty
    {
        /// <summary>
        /// Value that is checked when creating the flag.
        /// </summary>
        private readonly object _valueToIgnore;

        /// <summary>
        /// Default value of the property type. Intended for structs
        /// where null checks don't work.
        /// </summary>
        private readonly object _defaultValue;
        
        /// <summary>
        /// Creates the flag packet property.
        /// </summary>
        /// <param name="packetProperty">Packet property to wrap.</param>
        /// <param name="valueToIgnore">Value that results in the flag being read/written as false.</param>
        public FlagPacketProperty(IPacketProperty packetProperty, object valueToIgnore) : base(packetProperty)
        {
            this._valueToIgnore = valueToIgnore;
            this._defaultValue = this.Property.PropertyType.IsValueType ? Activator.CreateInstance(this.Property.PropertyType) : null;
        }
        
        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public override void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            var value = this.Property.GetValue(objectToWrite);
            if (object.Equals(value, this._valueToIgnore) || object.Equals(value?.ToString(), this._valueToIgnore) || (_valueToIgnore == null && object.Equals(value, this._defaultValue)) )
            {
                writer?.WriteBit(false);
            }
            else
            {
                writer?.WriteBit(true);
                this.WrappedProperty.Write(objectToWrite, writer, writtenProperties);
            }
        }

        /// <summary>
        /// Reads the property to the reader.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to read.</param>
        /// <param name="reader">Bit reader to read to.</param>
        /// <param name="readProperties">Properties that were previously read.</param>
        /// <param name="context">Properties that provide context for reading, such as world zone ids.</param>
        public override void Read(object objectToWrite, BitReader reader, Dictionary<string, object> readProperties, Dictionary<string, object> context)
        {
            // Read the value if the bit is set to true.
            if (reader == null || !reader.ReadBit())
            {
                if (_valueToIgnore == null || !this.Property.CanWrite) return;
                this.Property.SetValue(objectToWrite, Convert.ChangeType(_valueToIgnore, this.Property.PropertyType));
            }
            else
            {
                this.WrappedProperty.Read(objectToWrite, reader, readProperties, context);
            }
            
        }
    }
}