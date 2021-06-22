using System.IO;
using System.Numerics;
using NUnit.Framework;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure
{
    public class StructPacketParserTest
    {
        /// <summary>
        /// Packet with a single property.
        /// </summary>
        public struct SinglePropertyPacket
        {
            public int TestProperty1 { get; set; }
        }
        
        /// <summary>
        /// Packet with multiple properties.
        /// </summary>
        public struct MultiplePropertyPacket
        {
            public int TestProperty1 { get; set; }
            public int TestProperty2 { get; set; }
            public string TestProperty3 { get; set; }
            public string TestProperty4 { get; set; }
            public Quaternion TestProperty5 { get; set; }
            [NiQuaternion]
            public Quaternion TestProperty6 { get; set; }
        }
        
        /// <summary>
        /// Packet with Default.
        /// </summary>
        public struct DefaultPacket
        {
            [Default]
            public Vector3 TestProperty1 { get; set; }
            [Default("<0, 0, 0>")]
            public Vector3 TestProperty2 { get; set; }
            [Default]
            public int TestProperty3 { get; set; }
            [Default(4)]
            public int TestProperty4 { get; set; }
            public int TestProperty5 { get; set; }
        }
        
        /// <summary>
        /// Tests GetPacketProperties with only PacketProperties (no wrappers).
        /// </summary>
        [Test]
        public void TestGetPacketPropertiesPacketProperties()
        {
            // Test a packet with a single property.
            var singlePropertyPacket = StructPacketParser.GetPacketProperties(typeof(SinglePropertyPacket));
            Assert.AreEqual(singlePropertyPacket.Count, 1);
            Assert.IsTrue(singlePropertyPacket[0] is PacketProperty);
            Assert.AreEqual(singlePropertyPacket[0].Property, typeof(SinglePropertyPacket).GetProperty("TestProperty1"));
            
            // Test a packet with multiple properties.
            var multiplePropertyPacket = StructPacketParser.GetPacketProperties(typeof(MultiplePropertyPacket));
            Assert.AreEqual(multiplePropertyPacket.Count, 6);
            Assert.IsTrue(multiplePropertyPacket[0] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[0].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty1"));
            Assert.IsTrue(multiplePropertyPacket[1] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[1].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty2"));
            Assert.IsTrue(multiplePropertyPacket[2] is StringPacketProperty);
            Assert.AreEqual(multiplePropertyPacket[2].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty3"));
            Assert.IsTrue(multiplePropertyPacket[3] is StringPacketProperty);
            Assert.AreEqual(multiplePropertyPacket[3].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty4"));
            Assert.IsTrue(multiplePropertyPacket[4] is PacketProperty);
            Assert.AreEqual(multiplePropertyPacket[4].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty5"));
            Assert.IsTrue(multiplePropertyPacket[5] is NiQuaternionProperty);
            Assert.AreEqual(multiplePropertyPacket[5].Property, typeof(MultiplePropertyPacket).GetProperty("TestProperty6"));
        }
        
        /// <summary>
        /// Tests GetPacketProperties with defaults.
        /// </summary>
        [Test]
        public void TestGetPacketPropertiesDefaults()
        {
            var hasFlagPropertyPacket = StructPacketParser.GetPacketProperties(typeof(DefaultPacket));
            Assert.AreEqual(hasFlagPropertyPacket.Count, 5);
            Assert.IsTrue(hasFlagPropertyPacket[0] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[0].Property, typeof(DefaultPacket).GetProperty("TestProperty1"));
            Assert.IsTrue(hasFlagPropertyPacket[1] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[1].Property, typeof(DefaultPacket).GetProperty("TestProperty2"));
            Assert.IsTrue(hasFlagPropertyPacket[2] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[2].Property, typeof(DefaultPacket).GetProperty("TestProperty3"));
            Assert.IsTrue(hasFlagPropertyPacket[3] is FlagPacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[3].Property, typeof(DefaultPacket).GetProperty("TestProperty4"));
            Assert.IsTrue(hasFlagPropertyPacket[4] is PacketProperty);
            Assert.AreEqual(hasFlagPropertyPacket[4].Property, typeof(DefaultPacket).GetProperty("TestProperty5"));
        }

        /// <summary>
        /// Tests WritePacket.
        /// </summary>
        [Test]
        public void TestWritePacket()
        {
            // Create the test packet.
            var packet = new DefaultPacket()
            {
                TestProperty1 = Vector3.One,
                TestProperty2 = Vector3.Zero,
                TestProperty3 = 4,
                TestProperty4 = 4,
                TestProperty5 = 4,
            };
            
            // Test writing the packet.
            var reader = new BitReader(StructPacketParser.WritePacket(packet));
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<Vector3>(), Vector3.One);
            Assert.IsFalse(reader.ReadBit());
            Assert.IsTrue(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 4);
            Assert.IsFalse(reader.ReadBit());
            Assert.AreEqual(reader.Read<int>(), 4);
        }
        
        /// <summary>
        /// Tests ReadPacket.
        /// </summary>
        [Test]
        public void TestReadPacket()
        {
            // Write the data.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<Vector3>(Vector3.One);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(2);
            bitWriter.WriteBit(false);
            bitWriter.Write<int>(3);

            // Test reading the packet.
            var packet = (DefaultPacket) StructPacketParser.ReadPacket(typeof(DefaultPacket), stream);
            Assert.AreEqual(packet.TestProperty1, Vector3.One);
            Assert.AreEqual(packet.TestProperty2, Vector3.One);
            Assert.AreEqual(packet.TestProperty3, 2);
            Assert.AreEqual(packet.TestProperty4, 4);
            Assert.AreEqual(packet.TestProperty5, 3);
        }
    }
}