using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using RakDotNet.IO;

namespace Uchu.Core.Test.Serializable.Structure.Property
{
    public enum TestRequiredEnum
    {
        TestValue1,
        TestValue2,
        TestValue3,
    }
    
    public class TestRequiredProperties
    {
        public int TestProperty1 { get; set; }
        public bool TestProperty2 { get; set; }
        public TestRequiredEnum TestProperty3 { get; set; }
    }
    
    public class RequiredPacketPropertyTest
    {
        /// <summary>
        /// Tests writing required properties.
        /// </summary>
        [Test]
        public void TestWrite()
        {
            // Create the test properties.
            var mockPacketProperty = new MockPacketProperty(typeof(TestRequiredProperties).GetProperty("TestProperty1"));
            var requiredPacketProperty = new RequiredPacketProperty(mockPacketProperty, "TestProperty");
            var bitWriter = new BitWriter(new MemoryStream());

            // Test writing single properties options.
            requiredPacketProperty.RequiredValues.Add(2);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>());
            Assert.IsFalse(mockPacketProperty.DataWritten);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", 1},
            });
            Assert.IsFalse(mockPacketProperty.DataWritten);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", 2},
            });
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            requiredPacketProperty.RequiredValues[0] = true;
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", false},
            });
            Assert.IsFalse(mockPacketProperty.DataWritten);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", true},
            });
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            requiredPacketProperty.RequiredValues[0] = TestRequiredEnum.TestValue3;
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue1},
            });
            Assert.IsFalse(mockPacketProperty.DataWritten);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue3},
            });
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            // Test writing multiple properties options.
            requiredPacketProperty.RequiredValues[0] = 2;
            requiredPacketProperty.RequiredValues.Add(3);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", 1},
            });
            Assert.IsFalse(mockPacketProperty.DataWritten);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", 2},
            });
            Assert.IsTrue(mockPacketProperty.DataWritten);
            mockPacketProperty.DataWritten = false;
            
            requiredPacketProperty.RequiredValues[0] = TestRequiredEnum.TestValue2;
            requiredPacketProperty.RequiredValues[0] = TestRequiredEnum.TestValue3;
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue1},
            });
            Assert.IsFalse(mockPacketProperty.DataWritten);
            requiredPacketProperty.Write(mockPacketProperty, bitWriter, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue3},
            });
            Assert.IsTrue(mockPacketProperty.DataWritten);
        }
        
        /// <summary>
        /// Tests reading required properties.
        /// </summary>
        [Test]
        public void TestRead()
        {
            // Create the test properties.
            var mockPacketProperty = new MockPacketProperty(typeof(TestRequiredProperties).GetProperty("TestProperty1"));
            var requiredPacketProperty = new RequiredPacketProperty(mockPacketProperty, "TestProperty");
            var bitReader = new BitReader(new MemoryStream());

            // Test writing single properties options.
            requiredPacketProperty.RequiredValues.Add(2);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>(), null);
            Assert.IsFalse(mockPacketProperty.DataRead);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", 1},
            }, null);
            Assert.IsFalse(mockPacketProperty.DataRead);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", 2},
            }, null);
            Assert.IsTrue(mockPacketProperty.DataRead);
            mockPacketProperty.DataRead = false;
            
            requiredPacketProperty.RequiredValues[0] = true;
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", false},
            }, null);
            Assert.IsFalse(mockPacketProperty.DataRead);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", true},
            }, null);
            Assert.IsTrue(mockPacketProperty.DataRead);
            mockPacketProperty.DataRead = false;
            
            requiredPacketProperty.RequiredValues[0] = TestRequiredEnum.TestValue3;
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue1},
            }, null);
            Assert.IsFalse(mockPacketProperty.DataRead);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue3},
            }, null);
            Assert.IsTrue(mockPacketProperty.DataRead);
            mockPacketProperty.DataRead = false;
            
            // Test writing multiple properties options.
            requiredPacketProperty.RequiredValues[0] = 2;
            requiredPacketProperty.RequiredValues.Add(3);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", 1},
            }, null);
            Assert.IsFalse(mockPacketProperty.DataRead);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", 2},
            }, null);
            Assert.IsTrue(mockPacketProperty.DataRead);
            mockPacketProperty.DataRead = false;
            
            requiredPacketProperty.RequiredValues[0] = TestRequiredEnum.TestValue2;
            requiredPacketProperty.RequiredValues[0] = TestRequiredEnum.TestValue3;
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue1},
            }, null);
            Assert.IsFalse(mockPacketProperty.DataRead);
            requiredPacketProperty.Read(mockPacketProperty, bitReader, new Dictionary<string, object>()
            {
                { "TestProperty", TestRequiredEnum.TestValue3},
            }, null);
            Assert.IsTrue(mockPacketProperty.DataRead);
        }
    }
}