using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Compression.Zip.Tests
{
    [TestClass]
    public class ZipTest
    {
        private static readonly string TestZip = Path.Combine("..", "..", "..", "test.zip");
        private const string TestFile = "test.txt";

        [TestMethod]
        public void GetFile()
        {
            using (var reader = new StreamReader(Decompressor.GetFile(TestZip, TestFile)))
            {
                var text = reader.ReadToEnd();

                Assert.AreEqual("test", text);
            }
        }

        [TestMethod]
        public void DecompressDirectory()
        {
            var outDir = Path.GetTempPath();
            Decompressor.Decompress(TestZip, outDir);
            var outFile = Path.Combine(outDir, TestFile);
            var text = File.ReadAllText(outFile);
            Assert.AreEqual("test", text);
        }
    }
}