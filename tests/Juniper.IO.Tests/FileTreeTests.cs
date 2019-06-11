using System.IO;
using System.Linq;

using Juniper.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Compression.Tar.GZip.Tests
{
    [TestClass]
    public class FileTreeTests
    {
        private const string TEST_FILE = @"C:\Users\sean\Projects\Packages\uPromise.unitypackage";

        [TestMethod]
        public void FindPackages()
        {
            var packages = Decompressor.UnityPackageFiles(Path.GetDirectoryName(TEST_FILE));
            Assert.IsTrue(packages.Contains(TEST_FILE));
        }

        [TestMethod]
        public void ParseFileTree()
        {
            var tree = Decompressor.UnityPackageTree(TEST_FILE);
            Assert.AreEqual("Assets", tree.PathName);
            Assert.AreEqual("Assets\\uPromise", ((FileTree)tree.children[0]).PathName);
        }
    }
}
