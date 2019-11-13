using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{
    [TestClass]
    public class RouteTests
    {
        [TestMethod]
        public void ValidShortRoute()
        {
            Assert.IsNotNull(new Route<int>(1, 2, 3));
        }

        [TestMethod]
        public void InvalidShortRoute()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2));
        }

        [TestMethod]
        public void ValidMediumRoute()
        {
            Assert.IsNotNull(new Route<int>(1, 2, 3, 5));
        }

        [TestMethod]
        public void InvalidMediumRoutes()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 3));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 3, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 2));
        }

        [TestMethod]
        public void ValidLongRoute()
        {
            Assert.IsNotNull(new Route<int>(1, 2, 3, 5, 7));
        }

        [TestMethod]
        public void InvalidLongRoutes()
        {
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 2, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 2, 3));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 3, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 2, 3, 2, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 2, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 2, 3));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 3, 2));
            Assert.ThrowsException<InvalidOperationException>(() =>
                new Route<int>(1, 3, 3, 2, 2));
        }

        [TestMethod]
        public void ListContainsReversedRoute()
        {
            var list = new List<Route<int>>();
            var route = new Route<int>(5, 0xbad, 0xf00d);
            list.Add(route);
            var reverse = ~route;
            Assert.IsTrue(list.Contains(route));
            Assert.IsTrue(list.Contains(reverse));
        }

        [TestMethod]
        public void ListRemoveReversedRoute()
        {
            var list = new List<Route<int>>();
            var route = new Route<int>(5, 0xbad, 0xf00d);
            list.Add(route);
            var reverse = ~route;
            list.Remove(reverse);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void DistinctRoutes()
        {
            var route = new Route<int>(5, 0xbad, 0xf00d);
            var routes = new[]
            {
                route,
                ~route
            };

            var distinctRoutes = routes.Distinct().ToArray();

            Assert.AreEqual(routes[0], routes[1]);
            Assert.AreEqual(1, distinctRoutes.Length);
            Assert.AreEqual(route, distinctRoutes[0]);
        }

        [TestMethod]
        public void ConcatShortRoutes()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xdead);
            var routeC = ~routeA;
            var routeD = ~routeB;

            var a = routeA + routeB;
            var b = routeA + routeD;
            var c = routeB + routeA;
            var d = routeB + routeC;

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreEqual(b, c);
            Assert.AreEqual(b, d);
            Assert.AreEqual(c, d);
        }

        [TestMethod]
        public void ConcatMediumRoutes()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead);
            var routeB = new Route<int>(1, 0xdead, 0xf00f, 0x1337);
            var routeC = ~routeA;
            var routeD = ~routeB;

            var a = routeA + routeB;
            var b = routeA + routeD;
            var c = routeB + routeA;
            var d = routeB + routeC;

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreEqual(b, c);
            Assert.AreEqual(b, d);
            Assert.AreEqual(c, d);
        }

        [TestMethod]
        public void ConcatLongRoutes()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead, 0xf00f);
            var routeB = new Route<int>(1, 0xf00f, 0x1337, 0x4444, 0xaaaa);
            var routeC = ~routeA;
            var routeD = ~routeB;

            var a = routeA + routeB;
            var b = routeA + routeD;
            var c = routeB + routeA;
            var d = routeB + routeC;

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreEqual(b, c);
            Assert.AreEqual(b, d);
            Assert.AreEqual(c, d);
        }

        [TestMethod]
        public void SinglePointIntersect()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead);
            var routeB = new Route<int>(1, 0xdead, 0xbeef, 0x1337);
            Assert.IsTrue(routeA.Intersects(routeB));
            Assert.IsTrue(routeB.Intersects(routeA));
        }

        [TestMethod]
        public void TwoPointIntersect()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead, 0xf00f);
            var routeB = new Route<int>(1, 0xf00f, 0xbeef, 0xdead, 0xaaaa);
            Assert.IsTrue(routeA.Intersects(routeB));
            Assert.IsTrue(routeB.Intersects(routeA));
        }

        [TestMethod]
        public void ConnectionContainsConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbad, 0xbeef);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ReverseConnectionContainsConnection()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbad, 0xbeef);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionContainsReverseConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xbad, 0xbeef);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionDoesNotContainConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ReverseConnectionDoesNotContainConnection()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionDoesNotContainReverseConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionContainsPath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ReverseConnectionContainsPath3()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionContainsReversePath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xf00f, 0xbad, 0xbeef);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionContainsPath4()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef, 0x1337);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ReverseConnectionContainsPath4()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef, 0x1337);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ConnectionContainsReversePath4()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xf00f, 0xbad, 0xbeef, 0x1337);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void Path3ContainsPath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ReversePath3ContainsPath3()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void Path3ContainsReversePath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            var routeB = ~new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [TestMethod]
        public void Path3DoesntContainPath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf003);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [TestMethod]
        public void ReversePath3DoesntContainPath3()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef, 0xf003);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [TestMethod]
        public void Path3DoesntContainReversePath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf003);
            var routeB = ~new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }
    }
}
