using RakDotNet;
using Uchu.Core;

namespace Uchu.World
{
    public class WorldPacketStruct : PacketStruct
    {
        /// <summary>
        /// Registers the World-specific writers and readers.
        /// </summary>
        static WorldPacketStruct()
        {
            // Add Lot.
            PacketProperty.CustomWriters[typeof(Lot)] = (writer, o) =>
            {
                writer.Write(((Lot) o).Id);
            };
            PacketProperty.CustomReaders[typeof(Lot)] = (reader, context) =>
            {
                return new Lot(reader.Read<int>());
            };
            
            // Add GameObject.
            PacketProperty.CustomWriters[typeof(GameObject)] = (writer, o) =>
            {
                writer.Write((GameObject) o);
            };
            PacketProperty.CustomReaders[typeof(GameObject)] = (reader, context) =>
            {
                return reader.ReadGameObject((Zone) context["Zone"]);
            };
        }
        
        /// <summary>
        /// Creates the packet struct attribute for a Server Game Message.
        /// </summary>
        /// <param name="messageIdentifier">Message identifier of the packet.</param>
        /// <param name="remoteConnectionType">Remote connection type of the packet.</param>
        /// <param name="packetId">Id of the packet.</param>
        public WorldPacketStruct(MessageIdentifier messageIdentifier,RemoteConnectionType remoteConnectionType, uint packetId) : base(messageIdentifier, remoteConnectionType, packetId)
        {
            
        }
    }
}