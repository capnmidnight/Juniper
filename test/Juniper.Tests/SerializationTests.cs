using NUnit.Framework;

using System.Collections.Generic;
using System.IO;

namespace Juniper.IO
{
    [TestFixture]
    public class SerializationTests
    {
        public static void DictionaryTest<T>()
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
            foreach (var key in expectedDict.Keys)
            {
                Assert.AreEqual(expectedDict[key], actualDict[key]);
            }
        }

        [Test]
        public void TestJsonDictionary()
        {
            DictionaryTest<JsonFactory<Dictionary<string, int>>>();
        }

        [TestCase]
        public void SerializeToFile()
        {
            const string fileName = "testSerializationToFile.json";

            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var factory = new JsonFactory<string[]>();
            var values = new[]
            {
                "asdf",
                "qwer",
                "zxcv"
            };

            factory.Serialize(fileName, values);

            Assert.IsTrue(File.Exists(fileName));
        }
    }
}
