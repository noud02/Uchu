using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure.Property
{
    public class TestStrings
    {
        public string TestString1 { get; set; }
        [Length(10)]
        public string TestString2 { get; set; }
        [Wide]
        public string TestString3 { get; set; }
        [Wide]
        [Length(10)]
        public string TestString4 { get; set; }
    }
    
    public class StringPacketPropertyTest
    {
        /// <summary>
        /// Tests writing strings.
        /// </summary>
        [Test]
        public void TestWrite()
        {
            // Create the test strings.
            var testStrings = new TestStrings()
            {
                TestString1 = "Test1",
                TestString2 = "Test2",
                TestString3 = "Test3",
                TestString4 = "Test4",
            };
            var testStringsType = testStrings.GetType();
            
            // Create the test BitWriter.
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            
            // Write the properties to the BitWriter and assert the expected data was written.
            var writtenProperties = new Dictionary<string, object>();
            new StringPacketProperty(testStringsType.GetProperty("TestString1")).Write(testStrings, bitWriter, writtenProperties);
            new StringPacketProperty(testStringsType.GetProperty("TestString2")).Write(testStrings, bitWriter, writtenProperties);
            new StringPacketProperty(testStringsType.GetProperty("TestString3")).Write(testStrings, bitWriter, writtenProperties);
            new StringPacketProperty(testStringsType.GetProperty("TestString4")).Write(testStrings, bitWriter, writtenProperties);
            Assert.AreEqual(writtenProperties, new Dictionary<string, object>()
            {
                {"TestString1", "Test1"},
                {"TestString2", "Test2"},
                {"TestString3", "Test3"},
                {"TestString4", "Test4"},
            });
            
            // Assert the strings were written.
            var bitReader = new BitReader(stream);
            Assert.AreEqual(bitReader.Read<uint>(), 5);
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 'T');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 'e');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 's');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 't');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) '1');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 'T');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 'e');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 's');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) 't');
            Assert.AreEqual(bitReader.Read<byte>(), (byte) '2');
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(bitReader.Read<byte>(), 0);
            }
            Assert.AreEqual(bitReader.Read<uint>(), 5);
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 'T');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 'e');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 's');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 't');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) '3');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 'T');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 'e');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 's');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) 't');
            Assert.AreEqual(bitReader.Read<ushort>(), (ushort) '4');
            for (var i = 0; i < 5; i++)
            {
                Assert.AreEqual(bitReader.Read<ushort>(), 0);
            }
        }
        
        /// <summary>
        /// Tests reading strings.
        /// </summary>
        [Test]
        public void TestRead()
        {
            // Create the initial data.
            var testStrings = new TestStrings();
            var testStringsType = testStrings.GetType();
            var stream = new MemoryStream();
            var bitWriter = new BitWriter(stream);
            bitWriter.Write<uint>(5);
            bitWriter.Write<byte>((byte) 'T');
            bitWriter.Write<byte>((byte) 'e');
            bitWriter.Write<byte>((byte) 's');
            bitWriter.Write<byte>((byte) 't');
            bitWriter.Write<byte>((byte) '5');
            bitWriter.Write<byte>((byte) 'T');
            bitWriter.Write<byte>((byte) 'e');
            bitWriter.Write<byte>((byte) 's');
            bitWriter.Write<byte>((byte) 't');
            bitWriter.Write<byte>((byte) '6');
            for (var i = 0; i < 5; i++)
            { 
                bitWriter.Write<byte>(0);
            }
            bitWriter.Write<uint>(5);
            bitWriter.Write<ushort>((ushort) 'T');
            bitWriter.Write<ushort>((ushort) 'e');
            bitWriter.Write<ushort>((ushort) 's');
            bitWriter.Write<ushort>((ushort) 't');
            bitWriter.Write<ushort>((ushort) '7');
            bitWriter.Write<ushort>((ushort) 'T');
            bitWriter.Write<ushort>((ushort) 'e');
            bitWriter.Write<ushort>((ushort) 's');
            bitWriter.Write<ushort>((ushort) 't');
            bitWriter.Write<ushort>((ushort) '8');
            for (var i = 0; i < 5; i++)
            { 
                bitWriter.Write<ushort>(0);
            }
            
            // REad the properties from the BitReader and assert the expected properties was read.
            var readProperties = new Dictionary<string, object>();
            var bitReader = new BitReader(stream);
            new StringPacketProperty(testStringsType.GetProperty("TestString1")).Read(testStrings, bitReader, readProperties, null);
            new StringPacketProperty(testStringsType.GetProperty("TestString2")).Read(testStrings, bitReader, readProperties, null);
            new StringPacketProperty(testStringsType.GetProperty("TestString3")).Read(testStrings, bitReader, readProperties, null);
            new StringPacketProperty(testStringsType.GetProperty("TestString4")).Read(testStrings, bitReader, readProperties, null);
            Assert.AreEqual(testStrings.TestString1, "Test5");
            Assert.AreEqual(testStrings.TestString2, "Test6");
            Assert.AreEqual(testStrings.TestString3, "Test7");
            Assert.AreEqual(testStrings.TestString4, "Test8");
            Assert.AreEqual(readProperties, new Dictionary<string, object>()
            {
                {"TestString1", "Test5"},
                {"TestString2", "Test6"},
                {"TestString3", "Test7"},
                {"TestString4", "Test8"},
            });
        }
    }
}