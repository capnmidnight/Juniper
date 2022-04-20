using NUnit.Framework;

using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Juniper.Compression.Zip
{
    [TestFixture]
    public class ZipTest
    {
        private const string TestFile1 = "test.txt";
        private const string TestFile2 = "test2.txt";

        private static readonly string[] TestFiles = { TestFile1, TestFile2 };

        [Test]
        public void GetFile()
        {
            using var mem = new MemoryStream(Juniper.Tests.Properties.Resources.test_zip);
            using var reader = new StreamReader(Decompressor.GetFile(mem, TestFile1));
            var text = reader.ReadToEnd();

            Assert.AreEqual("test", text);
        }

        [Test]
        public void DecompressDirectory()
        {
            var outDir = Path.GetTempPath();
            using var mem = new MemoryStream(Juniper.Tests.Properties.Resources.test_zip);
            Decompressor.Decompress(mem, outDir);
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
            using var mem = new MemoryStream(Juniper.Tests.Properties.Resources.test_zip);
            var fileNames = Decompressor.Entries(mem)
                .Files()
                .Names()
                .ToArray();
            Assert.IsTrue(fileNames.Matches(TestFiles));
        }
    }
}