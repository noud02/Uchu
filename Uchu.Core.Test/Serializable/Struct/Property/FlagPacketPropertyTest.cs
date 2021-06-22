using System.Collections.Generic;
using System.IO;
using System.Numerics;
using NUnit.Framework;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure.Property
{
    public enum TestFlagEnum
    {
        TestValue1,
        TestValue2,
        TestValue3,
    }
    
    public class TestFlagProperties
    {
        public int TestProperty1 { get; set; }
        public Vector3 TestProperty2 { get; set; }
        public TestFlagEnum TestProperty3 { get; set; }
        public TestFlagProperties TestProperty4 { get; set; }
    }
    
    public class FlagPacketPropertyTest
    {
        /// <summary>
        /// Tests writing constant properties.
        /// </summary>
        [Test]
        public void TestWriteConstantProperty()
        {
            // Test writing with the value to write being ignored being null.
            var testProperties = new TestFlagProperties();
            var mockPacketProperty = new MockPacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty1"));
            var flagPacketProperty = new FlagPacketProperty(mockPacketProperty, null);
            var writtenProperties = new Dictionary<string, object>();
            var bitWriter = new BitWriter(new MemoryStream());
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty1 = 2;
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            // Test writing with the value to write being ignored being defined.
            flagPacketProperty = new FlagPacketProperty(mockPacketProperty, 2);
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty1 = 3;
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
        }
        
        /// <summary>
        /// Tests writing non-constant properties.
        /// This is intended for annotations that can only store constant types, so
        /// to ToString() result will be relied on.
        /// </summary>
        [Test]
        public void TestWriteNonConstantProperty()
        {
            // Test writing with the value to write being ignored being null.
            var testProperties = new TestFlagProperties();
            var mockPacketProperty = new MockPacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty2"));
            var flagPacketProperty = new FlagPacketProperty(mockPacketProperty, null);
            var writtenProperties = new Dictionary<string, object>();
            var bitWriter = new BitWriter(new MemoryStream());
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty2 = new Vector3(2, 2, 2);
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            // Test writing with the value to write being ignored being defined.
            flagPacketProperty = new FlagPacketProperty(mockPacketProperty, "<2, 2, 2>");
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty2 = new Vector3(3,3,3);
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
        }
        
        /// <summary>
        /// Tests writing Enum properties.
        /// </summary>
        [Test]
        public void TestWriteEnum()
        {
            // Test writing with the value to write being ignored being null.
            var testProperties = new TestFlagProperties();
            var mockPacketProperty = new MockPacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty3"));
            var flagPacketProperty = new FlagPacketProperty(mockPacketProperty, null);
            var writtenProperties = new Dictionary<string, object>();
            var bitWriter = new BitWriter(new MemoryStream());
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty3 = TestFlagEnum.TestValue3;
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            // Test writing with the value to write being ignored being defined.
            flagPacketProperty = new FlagPacketProperty(mockPacketProperty, TestFlagEnum.TestValue3);
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty3 = TestFlagEnum.TestValue2;
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
        }
        
        /// <summary>
        /// Tests writing object properties.
        /// </summary>
        [Test]
        public void TestWriteObject()
        {
            // Test writing with the value to write being ignored being null.
            // ToString cases are tested to a degree with non-constant structs.
            var testProperties = new TestFlagProperties();
            var mockPacketProperty = new MockPacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty4"));
            var flagPacketProperty = new FlagPacketProperty(mockPacketProperty, null);
            var writtenProperties = new Dictionary<string, object>();
            var bitWriter = new BitWriter(new MemoryStream());
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsFalse(mockPacketProperty.DataWritten);
            testProperties.TestProperty4 = new TestFlagProperties();
            flagPacketProperty.Write(testProperties, bitWriter, writtenProperties);
            Assert.IsTrue(mockPacketProperty.DataWritten);
        }

        /// <summary>
        /// Tests reading properties with flags.
        /// </summary>
        [Test]
        public void TestRead()
        {
            // Write the test data.
            var memoryStream = new MemoryStream();
            var bitWriter = new BitWriter(memoryStream);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(-5);
            bitWriter.WriteBit(false);
            bitWriter.WriteBit(true);
            bitWriter.Write<int>(1);
            bitWriter.WriteBit(false);
            
            // Test reading the properties with flags.
            var testProperties = new TestFlagProperties();
            var bitReader = new BitReader(memoryStream);
            var readProperties = new Dictionary<string, object>();
            new FlagPacketProperty(new PacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty1")), null)
                .Read(testProperties, bitReader, readProperties, null);
            new FlagPacketProperty(new PacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty2")), null)
                .Read(testProperties, bitReader, readProperties, null);
            new FlagPacketProperty(new PacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty3")), null)
                .Read(testProperties, bitReader, readProperties, null);
            new FlagPacketProperty(new PacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty4")), null)
                .Read(testProperties, bitReader, readProperties, null);
            Assert.AreEqual(testProperties.TestProperty1, -5);
            Assert.AreEqual(testProperties.TestProperty2, Vector3.Zero);
            Assert.AreEqual(testProperties.TestProperty3, TestFlagEnum.TestValue2);
            Assert.IsNull(testProperties.TestProperty4);
            Assert.AreEqual(readProperties, new Dictionary<string, object>()
            {
                {"TestProperty1", -5},
                {"TestProperty3", TestFlagEnum.TestValue2},
            });
        }
        
        /// <summary>
        /// Tests reading properties with defaults.
        /// </summary>
        [Test]
        public void TestReadDefaultValue()
        {
            // Write the test data.
            var memoryStream = new MemoryStream();
            var bitWriter = new BitWriter(memoryStream);
            bitWriter.WriteBit(false);
            
            // Test reading the properties with flags.
            var testProperties = new TestFlagProperties();
            var bitReader = new BitReader(memoryStream);
            var readProperties = new Dictionary<string, object>();
            new FlagPacketProperty(new PacketProperty(typeof(TestFlagProperties).GetProperty("TestProperty1")), -5)
                .Read(testProperties, bitReader, readProperties, null);
            Assert.AreEqual(testProperties.TestProperty1, -5);
        }
    }
}