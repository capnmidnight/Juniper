using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Compression.Zip.Tests
{
    [TestClass]
    public class ZipTest
    {
        private static readonly string Root = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static readonly string TestZip = Root + "\\Projects\\Juniper\\src\\Juniper\\Assets\\Juniper\\ThirdParty\\Optional\\PicoMobileSDK.zip";
        private const string TestFile = "PicoMobileSDK/Pvr_Audio3D/API/Pvr_Audio3DAPI.cs";
        private const long TestFileLength = 6792;

        private static void Reject(Exception exp)
        {
            Assert.Fail(exp.Message);
        }

        [TestMethod]
        public void GetFile()
        {
            using (var reader = new StreamReader(Decompressor.GetFile(TestZip, TestFile)))
            {
                var text = reader.ReadToEnd();

                Assert.AreEqual(TestFileLength, text.Length);
            }
        }

        [TestMethod]
        public void DecompressDirectory()
        {
            var outDir = Path.GetTempPath();
            Decompressor.Decompress(TestZip, outDir, null, Reject);
            var outFile = Path.Combine(outDir, TestFile);
            var text = File.ReadAllText(outFile);
            Assert.AreEqual(TestFileLength, text.Length);
        }
    }
}