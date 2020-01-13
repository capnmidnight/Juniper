using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

namespace Juniper.Compression.Zip.Tests
{
    [TestFixture]
    public class ZipTest
    {
        private const string TestZip = "test.zip";
        private const string TestFile1 = "test.txt";
        private const string TestFile2 = "test2.txt";

        private static readonly string[] TestFiles = { TestFile1, TestFile2 };

        [Test]
        public void GetFile()
        {
            using var reader = new StreamReader(Decompressor.GetFile(TestZip, TestFile1));
            var text = reader.ReadToEnd();

            Assert.AreEqual("test", text);
        }

        [Test]
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

        [Test]
        public void FileNames()
        {
            var fileNames = Decompressor.Entries(TestZip)
                .Files()
                .Names()
                .ToArray();
            Assert.IsTrue(fileNames.Matches(TestFiles));
        }
    }
}