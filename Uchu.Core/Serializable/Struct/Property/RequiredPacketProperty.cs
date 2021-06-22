using System.Collections.Generic;
using System.Linq;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class RequiredPacketProperty : WrappedPacketProperty
    {
        /// <summary>
        /// Values for the property that are required to write.
        /// </summary>
        public readonly List<object> RequiredValues = new List<object>();

        /// <summary>
        /// Property that is checked.
        /// </summary>
        private readonly string _propertyToCheck;
        
        /// <summary>
        /// Creates the required packet property.
        /// </summary>
        /// <param name="packetProperty">Packet property to wrap.</param>
        /// <param name="propertyToCheck">Property that is checked.</param>
        public RequiredPacketProperty(IPacketProperty packetProperty, string propertyToCheck) : base(packetProperty)
        {
            this._propertyToCheck = propertyToCheck;
        }
        
        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public override void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            if (!writtenProperties.ContainsKey(this._propertyToCheck))
                return;

            if (!RequiredValues.Any(value => object.Equals(value, writtenProperties[this._propertyToCheck]))) return;
            this.WrappedProperty.Write(objectToWrite, writer, writtenProperties);
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
            if (!readProperties.ContainsKey(this._propertyToCheck))
                return;

            if (!RequiredValues.Any(value => object.Equals(value, readProperties[this._propertyToCheck]))) return;
            this.WrappedProperty.Read(objectToWrite, reader, readProperties, context);
        }
    }
}