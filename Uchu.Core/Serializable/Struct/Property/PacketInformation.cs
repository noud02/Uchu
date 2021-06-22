using System.Collections.Generic;
using System.Reflection;
using RakDotNet;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class PacketInformation : IPacketProperty
    {
        /// <summary>
        /// Property that is read from or read to.
        /// </summary>
        public PropertyInfo Property => null;
        
        /// <summary>
        /// Message identifier of the packet.
        /// </summary>
        private MessageIdentifier _messageIdentifier;

        /// <summary>
        /// Remote connection type of the packet.
        /// </summary>
        private RemoteConnectionType _remoteConnectionType;

        /// <summary>
        /// Id of the packet.
        /// </summary>
        private uint _packetId;

        /// <summary>
        /// Creates the packet information.
        /// </summary>
        /// <param name="messageIdentifier">Message identifier of the packet.</param>
        /// <param name="remoteConnectionType">Remote connection type of the packet.</param>
        /// <param name="packetId">Id of the packet.</param>
        public PacketInformation(MessageIdentifier messageIdentifier, RemoteConnectionType remoteConnectionType, uint packetId)
        {
            this._messageIdentifier = messageIdentifier;
            this._remoteConnectionType = remoteConnectionType;
            this._packetId = packetId;
        }

        /// <summary>
        /// Writes the property to the writer.
        /// </summary>
        /// <param name="objectToWrite">Object with the property to write.</param>
        /// <param name="writer">Bit writer to write to.</param>
        /// <param name="writtenProperties">Properties that were previously written.</param>
        public void Write(object objectToWrite, BitWriter writer, Dictionary<string, object> writtenProperties)
        {
            writer.Write((byte) this._messageIdentifier);
            writer.Write((ushort) this._remoteConnectionType);
            writer.Write(this._packetId);
            writer.Write<byte>(0);
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
            // The headers aren't used, so they are just read.
            reader.Read<byte>();
            reader.Read<ushort>();
            reader.Read<uint>();
            reader.Read<byte>();
        }
    }
}