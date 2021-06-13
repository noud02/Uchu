using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using RakDotNet;
using RakDotNet.IO;

namespace Uchu.Core
{
    public class StructPacketParser
    {
        /// <summary>
        /// Cache of the packet properties.
        /// These are compiled once and used for each read and write.
        /// </summary>
        private static readonly Dictionary<Type, List<IPacketProperty>> CachePacketProperties = new Dictionary<Type, List<IPacketProperty>>();

        /// <summary>
        /// Returns a list of packet properties for writing and reading packets.
        /// 
        /// </summary>
        /// <param name="type">Type to check the properties of.</param>
        /// <returns>List of properties to write.</returns>
        /// <exception cref="InvalidOperationException">Properties are invalid.</exception>
        public static List<IPacketProperty> GetPacketProperties(Type type)
        {
            // Populate the cache entry if needed.
            if (!CachePacketProperties.ContainsKey(type))
            {
                // Create the list to store the packet properties.
                var packetProperties = new List<IPacketProperty>();
                
                // Add the packet information.
                if (type.GetCustomAttribute(typeof(PacketStruct)) is PacketStruct packetStruct)
                {
                    packetProperties.Add(new PacketInformation(packetStruct.MessageIdentifier, packetStruct.RemoteConnectionType, packetStruct.PacketId));
                }
                
                // Convert the properties to PacketProperties.
                foreach (var property in type.GetProperties())
                {
                    // Create the base packet property.
                    IPacketProperty packetProperty = null;
                    if (property.PropertyType == typeof(string))
                    {
                        packetProperty = new StringPacketProperty(property);
                    }
                    else
                    {
                        packetProperty = new PacketProperty(property);
                    }
                    if ((property.GetCustomAttribute(typeof(Default)) is Default defaultAttribute))
                    {
                        packetProperty = new FlagPacketProperty(packetProperty, defaultAttribute.ValueToIgnore);
                    }
                    packetProperties.Add(packetProperty);
                }

                // Store the packet properties.
                CachePacketProperties[type] = packetProperties;
            }
            
            // Return the cache entry.
            return CachePacketProperties[type];
        }

        /// <summary>
        /// Writes the given packet struct to a memory stream.
        /// </summary>
        /// <param name="packet">Packet struct to write.</param>
        /// <typeparam name="T">Type of the packet.</typeparam>
        /// <returns></returns>
        public static MemoryStream WritePacket<T>(T packet) where T : struct
        {
            // Create the bit writer.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream, leaveOpen: true);

            // Write the properties.
            var writtenProperties = new Dictionary<string, object>();
            foreach (var property in GetPacketProperties(typeof(T)))
            {
                property.Write(packet, bitWriter, writtenProperties);
            }
            
            // Return the stream.
            bitWriter.Dispose();
            return stream;
        }
    }
}