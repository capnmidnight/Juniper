using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Data.Tests
{
    [TestClass]
    public class ZipTest
    {
        private const string TestZip = "C:\\Users\\smcbeth\\Projects\\Juniper\\src\\Juniper\\Assets\\Juniper\\ThirdParty\\Optional\\PicoMobileSDK.zip";
        private const string TestFile = "PicoMobileSDK/Pvr_Audio3D/API/Pvr_Audio3DAPI.cs";
        private const long TestFileLength = 6792;

        private static void Reject(Exception exp)
        {
            Assert.Fail(exp.Message);
        }

        [TestMethod]
        public void GetFile()
        {
            Zip.GetFile(
                TestZip,
                TestFile,
                stream =>
                {
                    using (var reader = new StreamReader(stream))
                    {
                        var text = reader.ReadToEnd();
                        Assert.AreEqual(TestFileLength, text.Length);
                    }
                },
                Reject);
        }

        [TestMethod]
        public void DecompressDirectory()
        {
            var outDir = Path.GetTempPath();
            Zip.DecompressDirectory(TestZip, outDir, null, Reject);
            var outFile = Path.Combine(outDir, TestFile);
            var text = File.ReadAllText(outFile);
            Assert.AreEqual(TestFileLength, text.Length);
        }
    }
}
