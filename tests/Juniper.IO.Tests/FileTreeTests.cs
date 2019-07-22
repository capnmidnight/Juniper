using System;
using System.IO;
using System.Linq;

using Juniper.Collections;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Compression.Tar.GZip.Tests
{
    [TestClass]
    public class FileTreeTests
    {
        private static readonly string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private static readonly string TEST_FILE = Path.Combine(appData, "Unity", "Asset Store-5.x", "Oculus", "ScriptingIntegration", "Oculus Integration.unitypackage");

        [TestMethod]
        public void FindPackages()
        {
            var packages = Decompressor.FindUnityPackages(Path.GetDirectoryName(TEST_FILE));
            Assert.IsTrue(packages.Contains(TEST_FILE));
        }

        [TestMethod]
        public void ParseFileTree()
        {
            var tree = Decompressor.UnityPackageTree(TEST_FILE);
            Assert.AreEqual("Assets", tree.PathName);
            Assert.AreEqual("Assets\\Oculus", ((FileTree)tree.children[0]).PathName);
        }
    }
}
