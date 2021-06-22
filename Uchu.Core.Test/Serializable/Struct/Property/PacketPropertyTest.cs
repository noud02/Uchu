using System.Collections.Generic;
using System.IO;
using System.Numerics;
using NUnit.Framework;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure.Property
{
    public enum TestEnum1
    {
        TestValue1,
        TestValue2,
        TestValue3,
    }
    
    public enum TestEnum2 : ushort
    {
        TestValue1,
        TestValue2,
        TestValue3,
    }
    
    public class TestProperties
    {
        public int TestProperty1 { get; set; }
        public uint TestProperty2 { get; set; }
        public float TestProperty3 { get; set; }
        public double TestProperty4 { get; set; }
        public Quaternion TestProperty5 { get; set; }
        public TestEnum1 TestProperty6 { get; set; }
        public TestEnum2 TestProperty7 { get; set; }
        public int[] TestProperty8 { get; set; }
        [StoreLengthAs(typeof(byte))]
        public int[] TestProperty9 { get; set; }
    }
    
    public class PacketPropertyTest
    {
        /// <summary>
        /// Tests writing normal properties.
        /// </summary>
        [Test]
        public void TestWriteNormalProperties()
        {
            // Create the test properties.
            var testProperties = new TestProperties()
            {
                TestProperty1 = -1,
                TestProperty2 = 2,
                TestProperty3 = -3,
                TestProperty4 = 4,
            };
            var testPropertiesType = testProperties.GetType();
            
            // Create the test BitWriter.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            
            // Write the properties to the BitWriter and assert the expected data was written.
            var writtenProperties = new Dictionary<string, object>();
            new PacketProperty(testPropertiesType.GetProperty("TestProperty1")).Write(testProperties, bitWriter, writtenProperties);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty2")).Write(testProperties, bitWriter, writtenProperties);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty3")).Write(testProperties, bitWriter, writtenProperties);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty4")).Write(testProperties, bitWriter, writtenProperties);
            var bitReader = new BitReader(stream);
            Assert.AreEqual(bitReader.Read<int>(), -1);
            Assert.AreEqual(bitReader.Read<uint>(), 2);
            Assert.AreEqual(bitReader.Read<float>(), -3);
            Assert.AreEqual(bitReader.Read<double>(), 4);
            Assert.AreEqual(writtenProperties, new Dictionary<string, object>()
            {
                {"TestProperty1", -1},
                {"TestProperty2", 2},
                {"TestProperty3", -3},
                {"TestProperty4", 4},
            });
        }

        /// <summary>
        /// Tests writing a Quaternion.
        /// </summary>
        [Test]
        public void TestWriteQuaternion()
        {
            // Create the test properties.
            var testProperties = new TestProperties()
            {
                TestProperty5 = new Quaternion(1,2,3,4),
            };
            var testPropertiesType = testProperties.GetType();
            
            // Create the test BitWriter.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            
            // Write the properties to the BitWriter and assert the expected data was written.
            var writtenProperties = new Dictionary<string, object>();
            new PacketProperty(testPropertiesType.GetProperty("TestProperty5")).Write(testProperties, bitWriter, writtenProperties);
            var bitReader = new BitReader(stream);
            Assert.AreEqual(bitReader.Read<float>(), 1);
            Assert.AreEqual(bitReader.Read<float>(), 2);
            Assert.AreEqual(bitReader.Read<float>(), 3);
            Assert.AreEqual(bitReader.Read<float>(), 4);
            Assert.AreEqual(writtenProperties, new Dictionary<string, object>()
            {
                {"TestProperty5", new Quaternion(1,2,3,4)},
            });
        }
        
        /// <summary>
        /// Tests writing enums.
        /// </summary>
        [Test]
        public void TestWriteEnum()
        {
            // Create the test properties.
            var testProperties = new TestProperties()
            {
                TestProperty6 = TestEnum1.TestValue3,
                TestProperty7 = TestEnum2.TestValue2,
            };
            var testPropertiesType = testProperties.GetType();
            
            // Create the test BitWriter.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            
            // Write the properties to the BitWriter and assert the expected data was written.
            var writtenProperties = new Dictionary<string, object>();
            new PacketProperty(testPropertiesType.GetProperty("TestProperty6")).Write(testProperties, bitWriter, writtenProperties);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty7")).Write(testProperties, bitWriter, writtenProperties);
            var bitReader = new BitReader(stream);
            Assert.AreEqual(bitReader.Read<int>(), 2);
            Assert.AreEqual(bitReader.Read<ushort>(), 1);
            Assert.AreEqual(writtenProperties, new Dictionary<string, object>()
            {
                {"TestProperty6", TestEnum1.TestValue3},
                {"TestProperty7", TestEnum2.TestValue2},
            });
        }
        
        /// <summary>
        /// Tests writing arrays.
        /// </summary>
        [Test]
        public void TestWriteArray()
        {
            // Create the test properties.
            var testProperties = new TestProperties()
            {
                TestProperty8 = new [] {1,2,3,4,5,6},
                TestProperty9 = new [] {1,2,3,4},
            };
            var testPropertiesType = testProperties.GetType();
            
            // Create the test BitWriter.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            
            // Write the properties to the BitWriter and assert the expected data was written.
            var writtenProperties = new Dictionary<string, object>();
            new PacketProperty(testPropertiesType.GetProperty("TestProperty8")).Write(testProperties, bitWriter, writtenProperties);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty9")).Write(testProperties, bitWriter, writtenProperties);
            var bitReader = new BitReader(stream);
            Assert.AreEqual(bitReader.Read<uint>(), 6);
            Assert.AreEqual(bitReader.Read<int>(), 1);
            Assert.AreEqual(bitReader.Read<int>(), 2);
            Assert.AreEqual(bitReader.Read<int>(), 3);
            Assert.AreEqual(bitReader.Read<int>(), 4);
            Assert.AreEqual(bitReader.Read<int>(), 5);
            Assert.AreEqual(bitReader.Read<int>(), 6);
            Assert.AreEqual(bitReader.Read<byte>(), 4);
            Assert.AreEqual(bitReader.Read<int>(), 1);
            Assert.AreEqual(bitReader.Read<int>(), 2);
            Assert.AreEqual(bitReader.Read<int>(), 3);
            Assert.AreEqual(bitReader.Read<int>(), 4);
            Assert.AreEqual(writtenProperties, new Dictionary<string, object>()
            {
                {"TestProperty8", new [] {1,2,3,4,5,6}},
                {"TestProperty9", new [] {1,2,3,4}},
            });
        }
        
        /// <summary>
        /// Tests reading normal properties.
        /// </summary>
        [Test]
        public void TestReadNormalProperties()
        {
            // Create the initial data.
            var testProperties = new TestProperties();
            var testPropertiesType = testProperties.GetType();
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.Write<int>(-5);
            bitWriter.Write<uint>(6);
            bitWriter.Write<float>(-7);
            bitWriter.Write<double>(8);

            // REad the properties from the BitReader and assert the expected properties was read.
            var readProperties = new Dictionary<string, object>();
            var bitReader = new BitReader(stream);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty1")).Read(testProperties, bitReader, readProperties, null);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty2")).Read(testProperties, bitReader, readProperties, null);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty3")).Read(testProperties, bitReader, readProperties, null);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty4")).Read(testProperties, bitReader, readProperties, null);
            Assert.AreEqual(testProperties.TestProperty1, -5);
            Assert.AreEqual(testProperties.TestProperty2, 6);
            Assert.AreEqual(testProperties.TestProperty3, -7);
            Assert.AreEqual(testProperties.TestProperty4, 8);
            Assert.AreEqual(readProperties, new Dictionary<string, object>()
            {
                {"TestProperty1", -5},
                {"TestProperty2", 6},
                {"TestProperty3", -7},
                {"TestProperty4", 8},
            });
        }

        /// <summary>
        /// Tests reading a Quaternion.
        /// </summary>
        [Test]
        public void TestReadQuaternion()
        {
            // Create the initial data.
            var testProperties = new TestProperties();
            var testPropertiesType = testProperties.GetType();
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.Write<float>(5);
            bitWriter.Write<float>(6);
            bitWriter.Write<float>(7);
            bitWriter.Write<float>(8);

            // Read the properties from the BitReader and assert the expected properties was read.
            var readProperties = new Dictionary<string, object>();
            var bitReader = new BitReader(stream);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty5")).Read(testProperties, bitReader, readProperties, null);
            Assert.AreEqual(testProperties.TestProperty5, new Quaternion(5,6,7,8));
            Assert.AreEqual(readProperties, new Dictionary<string, object>()
            {
                {"TestProperty5", new Quaternion(5,6,7,8)},
            });
        }
        
        /// <summary>
        /// Tests reading enums.
        /// </summary>
        [Test]
        public void TestReadEnum()
        {
            // Create the initial data.
            var testProperties = new TestProperties();
            var testPropertiesType = testProperties.GetType();
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.Write<int>(0);
            bitWriter.Write<ushort>(2);

            // Read the properties from the BitReader and assert the expected properties was read.
            var readProperties = new Dictionary<string, object>();
            var bitReader = new BitReader(stream);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty6")).Read(testProperties, bitReader, readProperties, null);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty7")).Read(testProperties, bitReader, readProperties, null);
            Assert.AreEqual(testProperties.TestProperty6, TestEnum1.TestValue1);
            Assert.AreEqual(testProperties.TestProperty7, TestEnum2.TestValue3);
            Assert.AreEqual(readProperties, new Dictionary<string, object>()
            {
                {"TestProperty6", TestEnum1.TestValue1},
                {"TestProperty7", TestEnum2.TestValue3},
            });
        }
        
        /// <summary>
        /// Tests reading arrays.
        /// </summary>
        [Test]
        public void TestReadArray()
        {
            // Create the initial data.
            var testProperties = new TestProperties();
            var testPropertiesType = testProperties.GetType();
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.Write<uint>(5);
            bitWriter.Write<int>(7);
            bitWriter.Write<int>(8);
            bitWriter.Write<int>(9);
            bitWriter.Write<int>(10);
            bitWriter.Write<int>(11);
            bitWriter.Write<byte>(3);
            bitWriter.Write<int>(12);
            bitWriter.Write<int>(13);
            bitWriter.Write<int>(14);

            // Read the properties from the BitReader and assert the expected properties was read.
            var readProperties = new Dictionary<string, object>();
            var bitReader = new BitReader(stream);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty8")).Read(testProperties, bitReader, readProperties, null);
            new PacketProperty(testPropertiesType.GetProperty("TestProperty9")).Read(testProperties, bitReader, readProperties, null);
            Assert.AreEqual(testProperties.TestProperty8, new int[] {7,8,9,10,11});
            Assert.AreEqual(testProperties.TestProperty9, new int[] {12,13,14});
            Assert.AreEqual(readProperties, new Dictionary<string, object>()
            {
                {"TestProperty8", new int[] {7,8,9,10,11}},
                {"TestProperty9", new int[] {12,13,14}},
            });
        }
    }
}