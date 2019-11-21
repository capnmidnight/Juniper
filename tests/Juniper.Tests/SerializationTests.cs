using System.Collections.Generic;
using System.Numerics;

using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Tests
{
    [TestClass]
    public class SerializationTests
    {        
        public void DictionaryTest<T>()
            where T : IFactory<Dictionary<string, int>, MediaType.Application>, new()
        {
            var factory = new T();
            var expectedDict = new Dictionary<string, int>
            {
                { "Michelle", 40 },
                { "Sean", 38 },
                { "Sheffield", 3 },
                { "Oxford", 2 }
            };

            var data = factory.Serialize(expectedDict);
            var actualDict = factory.Deserialize(data);

            Assert.AreNotSame(expectedDict, actualDict);
            Assert.IsTrue(actualDict.Keys.Matches(expectedDict.Keys));
            foreach(var key in expectedDict.Keys)
            {
                Assert.AreEqual(expectedDict[key], actualDict[key]);
            }
        }

        [TestMethod]
        public void TestBinaryDictionary()
        {
            DictionaryTest<BinaryFactory<Dictionary<string, int>>>();
        }

        [TestMethod]
        public void TestJsonDictionary()
        {
            DictionaryTest<JsonFactory<Dictionary<string, int>>>();
        }
    }
}
