using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Strings.Tests
{
    [TestClass]
    public class StringTests
    {
        [TestMethod]
        public void LevDistance1()
        {
            const string a = "hello";
            const string b = "jello";
            Assert.AreEqual(1, a.DistanceTo(b));
        }

        [TestMethod]
        public void LevDistance2()
        {
            const string a = "hello";
            const string b = "world";
            Assert.AreEqual(4, a.DistanceTo(b));
        }

        [TestMethod]
        public void LevDistance3()
        {
            const string a = "hello";
            const string b = "hello world";
            Assert.AreEqual(6, a.DistanceTo(b));
        }

        [TestMethod]
        public void LevDistance4()
        {
            const string a = "use gaze pointer";
            const string b = "use gays pointer";
            Assert.AreEqual(2, a.DistanceTo(b));
        }

        [TestMethod]
        public void Similarity1()
        {
            const string a = "hello";
            const string b = "hello";
            Assert.AreEqual(1, a.Similarity(b));
        }

        [TestMethod]
        public void Similarity2()
        {
            const string a = "hello";
            const string b = "jello";
            Assert.AreEqual(0.8667f, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void Similarity3()
        {
            const string a = "hello";
            const string b = "hello world";
            Assert.AreEqual(0.891, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void Similarity4()
        {
            const string a = "use gaze pointer";
            const string b = "use gays pointer";
            Assert.AreEqual(0.907, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void Similarity5()
        {
            const string a = "use gaze pointer";
            const string b = "use gay plotter";
            Assert.AreEqual(0.877, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void FixPath1()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var fixedPath = PathExt.FixPath(path);
            Assert.AreEqual(path, fixedPath);
        }

        [TestMethod]
        public void FixPath2()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var brokenPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var fixedPath = PathExt.FixPath(brokenPath);
            Assert.AreEqual(path, fixedPath);
        }
    }
}
