using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Strings.Tests
{
    [TestClass]
    public class StringTests
    {

        [TestMethod]
        public void LevDistance1()
        {
            var a = "hello";
            var b = "jello";
            Assert.AreEqual(1, a.DistanceTo(b));
        }

        [TestMethod]
        public void LevDistance2()
        {
            var a = "hello";
            var b = "world";
            Assert.AreEqual(4, a.DistanceTo(b));
        }

        [TestMethod]
        public void LevDistance3()
        {
            var a = "hello";
            var b = "hello world";
            Assert.AreEqual(6, a.DistanceTo(b));
        }

        [TestMethod]
        public void LevDistance4()
        {
            var a = "use gaze pointer";
            var b = "use gays pointer";
            Assert.AreEqual(2, a.DistanceTo(b));
        }

        [TestMethod]
        public void Similarity1()
        {
            var a = "hello";
            var b = "hello";
            Assert.AreEqual(1, a.Similarity(b));
        }

        [TestMethod]
        public void Similarity2()
        {
            var a = "hello";
            var b = "jello";
            Assert.AreEqual(0.8667f, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void Similarity3()
        {
            var a = "hello";
            var b = "hello world";
            Assert.AreEqual(0.891, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void Similarity4()
        {
            var a = "use gaze pointer";
            var b = "use gays pointer";
            Assert.AreEqual(0.907, a.Similarity(b), 0.001f);
        }

        [TestMethod]
        public void Similarity5()
        {
            var a = "use gaze pointer";
            var b = "use gay plotter";
            Assert.AreEqual(0.877, a.Similarity(b), 0.001f);
        }
    }
}
