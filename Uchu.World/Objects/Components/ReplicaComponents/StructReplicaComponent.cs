using System;
using System.Runtime.CompilerServices;
using RakDotNet.IO;
using Uchu.Core;

namespace Uchu.World
{
    public abstract class StructReplicaComponent<TConstructPacketType, TSerializePacketType> : ReplicaComponent
        where TConstructPacketType : struct
        where TSerializePacketType : struct
    {
        /// <summary>
        /// Returns the packet created by cloning the properties
        /// of the replica component to the given packet.
        /// </summary>
        /// <typeparam name="T">Packet type to create.</typeparam>
        /// <returns>Packet for the given type.</returns>
        public T GetPacket<T>() where T : struct
        {
            // Create the packet.
            // SetValue is a bit more complicated because the object is a struct,
            // which results in a new struct being created instead of the existing
            // one changing.
            var packet = Activator.CreateInstance<T>();
            var objectPacket = RuntimeHelpers.GetObjectValue(packet);
            
            // Copy the values to the packet.
            var packetType = packet.GetType();
            var replicateType = this.GetType();
            foreach (var packetProperty in packetType.GetProperties())
            {
                if (!packetProperty.CanWrite) continue;
                var replicateProperty = replicateType.GetProperty(packetProperty.Name);
                if (replicateProperty == null) continue;
                var value = replicateProperty.GetValue(this);
                packetProperty.SetValue(objectPacket, value);
            }
            
            // Return the packet.
            return (T) objectPacket;
        }
        
        /// <summary>
        /// Creates the Construct packet for the replica component.
        /// </summary>
        /// <returns>The Construct packet for the replica component.</returns>
        public virtual TConstructPacketType GetConstructPacket()
        {
            return this.GetPacket<TConstructPacketType>();
        }
        
        /// <summary>
        /// Creates the Serialize packet for the replica component.
        /// </summary>
        /// <returns>The Serialize packet for the replica component.</returns>
        public virtual TSerializePacketType GetSerializePacket()
        {
            return this.GetPacket<TSerializePacketType>();
        }

        /// <summary>
        /// The data that is only sent once to each client.
        /// </summary>
        /// <param name="writer"></param>
        public override void Construct(BitWriter writer)
        {
            StructPacketParser.WritePacket(this.GetConstructPacket(), writer);
        }

        /// <summary>
        /// The data that is sent every time an update accrues.
        /// </summary>
        /// <param name="writer"></param>
        public override void Serialize(BitWriter writer)
        {
            StructPacketParser.WritePacket(this.GetSerializePacket(), writer);
        }
    }

    public abstract class StructReplicaComponent<TPacketType> : StructReplicaComponent<TPacketType, TPacketType>
        where TPacketType : struct
    {
        /// <summary>
        /// Creates the packet for the replica component.
        /// </summary>
        /// <returns>The packet for the replica component.</returns>
        public virtual TPacketType GetPacket()
        {
            return this.GetPacket<TPacketType>();
        }
        
        /// <summary>
        /// Creates the Construct packet for the replica component.
        /// </summary>
        /// <returns>The Construct packet for the replica component.</returns>
        public override TPacketType GetConstructPacket()
        {
            return this.GetPacket();
        }
        
        /// <summary>
        /// Creates the Serialize packet for the replica component.
        /// </summary>
        /// <returns>The Serialize packet for the replica component.</returns>
        public override TPacketType GetSerializePacket()
        {
            return this.GetPacket();
        }
    }
}