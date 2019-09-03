using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Compression.Zip.Tests
{
    [TestClass]
    public class ZipTest
    {
        private static readonly string TestZip = Path.Combine("..", "..", "..", "test.zip");
        private const string TestFile1 = "test.txt";
        private const string TestFile2 = "test2.txt";

        private static readonly string[] TestFiles = { TestFile1, TestFile2 };

        [TestMethod]
        public void GetFile()
        {
            using (var reader = new StreamReader(Decompressor.GetFile(TestZip, TestFile1)))
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
            foreach (var testFile in TestFiles)
            {
                var outFile = Path.Combine(outDir, testFile);
                var text = File.ReadAllText(outFile);
                Assert.AreEqual(Path.GetFileNameWithoutExtension(testFile), text);
            }
        }
    }
}