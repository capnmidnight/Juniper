using NUnit.Framework;

using System;
using System.IO;

namespace Juniper.Strings
{
    [TestFixture]
    public class StringTests
    {
        [Test]
        public void LevDistance1()
        {
            const string a = "hello";
            const string b = "jello";
            Assert.AreEqual(1, a.DistanceTo(b));
        }

        [Test]
        public void LevDistance2()
        {
            const string a = "hello";
            const string b = "world";
            Assert.AreEqual(4, a.DistanceTo(b));
        }

        [Test]
        public void LevDistance3()
        {
            const string a = "hello";
            const string b = "hello world";
            Assert.AreEqual(6, a.DistanceTo(b));
        }

        [Test]
        public void LevDistance4()
        {
            const string a = "use gaze pointer";
            const string b = "use gays pointer";
            Assert.AreEqual(2, a.DistanceTo(b));
        }

        [Test]
        public void Similarity1()
        {
            const string a = "hello";
            const string b = "hello";
            Assert.AreEqual(1, a.Similarity(b));
        }

        [Test]
        public void Similarity2()
        {
            const string a = "hello";
            const string b = "jello";
            Assert.AreEqual(0.8667f, a.Similarity(b), 0.001f);
        }

        [Test]
        public void Similarity3()
        {
            const string a = "hello";
            const string b = "hello world";
            Assert.AreEqual(0.891, a.Similarity(b), 0.001f);
        }

        [Test]
        public void Similarity4()
        {
            const string a = "use gaze pointer";
            const string b = "use gays pointer";
            Assert.AreEqual(0.907, a.Similarity(b), 0.001f);
        }

        [Test]
        public void Similarity5()
        {
            const string a = "use gaze pointer";
            const string b = "use gay plotter";
            Assert.AreEqual(0.877, a.Similarity(b), 0.001f);
        }

        [Test]
        public void FixPath1()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var fixedPath = PathExt.FixPath(path);
            Assert.AreEqual(path, fixedPath);
        }

        [Test]
        public void FixPath2()
        {
            var path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var brokenPath = path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            Assert.AreNotEqual(path, brokenPath);
            var fixedPath = PathExt.FixPath(brokenPath);
            Assert.AreEqual(path, fixedPath);
        }
    }
}
