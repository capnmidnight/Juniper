using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Compression.Tests
{
    [TestClass]
    public class FileTreeTests
    {
        private static readonly string TestZip = Path.Combine("..", "..", "..", "test.zip");
        private const string TestFile1 = "test.txt";
        private const string TestFile2 = "test2.txt";
        private static readonly string[] TestFiles = { TestFile1, TestFile2 };

        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string TestTarGZ = Path.Combine(appData, "Unity", "Asset Store-5.x", "Oculus", "ScriptingIntegration", "Oculus Integration.unitypackage");

        [TestMethod]
        public void FindPackages()
        {
            var packages = Tar.GZip.Decompressor.FindUnityPackages(Path.GetDirectoryName(TestTarGZ));
            Assert.IsTrue(packages.Contains(TestTarGZ));
        }

        [TestMethod]
        public void GetEntries()
        {
            var entries = Tar.GZip.Decompressor.Entries(TestTarGZ).ToArray();
        }

        [TestMethod]
        public void BuildUnityPackageTree()
        {
            var tree = Tar.GZip.Decompressor.Entries(TestTarGZ).Tree();
        }

        [TestMethod]
        public void MakeTree()
        {
            var tree = Zip.Decompressor.Entries(TestZip).Tree();
            Assert.IsNotNull(tree);
            Assert.AreEqual(2, tree.Count);
            Assert.IsTrue(tree.children.Any(v => v.Value.FullName == TestFile1));
            Assert.IsTrue(tree.children.Any(v => v.Value.FullName == TestFile2));
        }
    }
}