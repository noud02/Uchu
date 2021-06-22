using System;
using System.IO;
using RakDotNet;
using RakDotNet.IO;
using Uchu.Core.Resources;

namespace Uchu.Core
{
    public static class RakConnectionExtensions
    {
        public static void Send(this IRakConnection @this, ISerializable serializable)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), 
                    ResourceStrings.RakConnectionExtensions_Send_ConnectionNullException);
            if (serializable == null)
                throw new ArgumentNullException(nameof(serializable), 
                    ResourceStrings.RakConnectionExtensions_Send_StreamNullException);
            Logger.Debug($"Sending {serializable}");
            
            using var stream = new MemoryStream();
            using var writer = new BitWriter(stream);
            
            writer.Write(serializable);

            try
            {
                @this.Send(stream.ToArray());
            }
            catch (IOException e)
            {
                Logger.Error(e);
            }
        }

        public static void Send(this IRakConnection @this, MemoryStream stream)
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this),  
                    ResourceStrings.RakConnectionExtensions_Send_ConnectionNullException);
            if (stream == null)
                throw new ArgumentNullException(nameof(stream), 
                    ResourceStrings.RakConnectionExtensions_Send_StreamNullException);
            
            @this.Send(stream.ToArray());
        }
        
        /// <summary>
        /// Sends a struct (packet) to a given connection.
        /// </summary>
        /// <param name="this">RakNet connection to send over.</param>
        /// <param name="packet">Packet to send.</param>
        /// <typeparam name="T">Type of the packet.</typeparam>
        /// <exception cref="ArgumentNullException">Connection is null.</exception>
        public static void Send<T>(this IRakConnection @this, T packet) where T : struct
        {
            if (@this == null)
                throw new ArgumentNullException(nameof(@this), 
                    ResourceStrings.RakConnectionExtensions_Send_ConnectionNullException);
            Logger.Debug($"Sending {packet}");
            
            try
            {
                @this.Send(StructPacketParser.WritePacket(packet).ToArray());
            }
            catch (IOException e)
            {
                Logger.Error(e);
            }
        }
    }
}