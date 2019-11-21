using System.Collections.Generic;
using System.Numerics;

using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Tests
{
    [TestClass]
    public class SerializationTests
    {
        [TestMethod]
        public void TestSerializeVector2()
        {
            var json = new JsonFactory<Vector2>();
            var v1 = new Vector2(3, 5);
            var text = json.ToString(v1);
            var v2 = json.Parse(text);
            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void TestSerializeVector3()
        {
            var json = new JsonFactory<Vector3>();
            var v1 = new Vector3(3, 5, 7);
            var text = json.ToString(v1);
            var v2 = json.Parse(text);
            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void TestSerializeVector4()
        {
            var json = new JsonFactory<Vector4>();
            var v1 = new Vector4(3, 5, 7, 11);
            var text = json.ToString(v1);
            var v2 = json.Parse(text);
            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void TestSerializeQuaternion()
        {
            var json = new JsonFactory<Quaternion>();
            var v1 = new Quaternion(3, 5, 7, 11);
            var text = json.ToString(v1);
            var v2 = json.Parse(text);
            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void TestSerializeMatrix3x2()
        {
            var json = new JsonFactory<Matrix3x2>();
            var v1 = new Matrix3x2(3, 5, 7, 11, 13, 17);
            var text = json.ToString(v1);
            var v2 = json.Parse(text);
            Assert.AreEqual(v1, v2);
        }

        [TestMethod]
        public void TestSerializeMatrix4x4()
        {
            var json = new JsonFactory<Matrix4x4>();
            var v1 = new Matrix4x4(3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59);
            var text = json.ToString(v1);
            var v2 = json.Parse(text);
            Assert.AreEqual(v1, v2);
        }

        
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
