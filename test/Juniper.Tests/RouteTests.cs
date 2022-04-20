using NUnit.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.Collections
{
    [TestFixture]
    public class RouteTests
    {
        [Test]
        public void ValidShortRoute()
        {
            Assert.IsNotNull(new Route<int>(1, 2, 3));
        }

        [Test]
        public void InvalidShortRoute()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2));
        }

        [Test]
        public void ValidMediumRoute()
        {
            Assert.IsNotNull(new Route<int>(1, 2, 3, 5));
        }

        [Test]
        public void InvalidMediumRoutes()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 3));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 3, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 2));
        }

        [Test]
        public void ValidLongRoute()
        {
            Assert.IsNotNull(new Route<int>(1, 2, 3, 5, 7));
        }

        [Test]
        public void InvalidLongRoutes()
        {
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 2, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 2, 3));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 2, 3, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 2, 3, 2, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 2, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 2, 3));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 3, 2, 3, 2));
            Assert.Throws<InvalidOperationException>(() =>
                new Route<int>(1, 3, 3, 2, 2));
        }

        [Test]
        public void IsOrdered()
        {
            var route = new Route<string>(1, "A", "B", "C", "D");
            Assert.IsFalse(route.Ordered("nothing", "A"));
            Assert.IsFalse(route.Ordered("A", "nothing"));
            Assert.IsFalse(route.Ordered("nothing", "nothing"));
            Assert.IsFalse(route.Ordered("A", "A"));
            Assert.IsTrue(route.Ordered("A", "B"));
            Assert.IsTrue(route.Ordered("A", "C"));
            Assert.IsTrue(route.Ordered("A", "D"));
            Assert.IsFalse(route.Ordered("B", "A"));
            Assert.IsFalse(route.Ordered("B", "B"));
            Assert.IsTrue(route.Ordered("B", "C"));
            Assert.IsTrue(route.Ordered("B", "D"));
            Assert.IsFalse(route.Ordered("C", "A"));
            Assert.IsFalse(route.Ordered("C", "B"));
            Assert.IsFalse(route.Ordered("C", "C"));
            Assert.IsTrue(route.Ordered("C", "D"));
            Assert.IsFalse(route.Ordered("D", "A"));
            Assert.IsFalse(route.Ordered("D", "B"));
            Assert.IsFalse(route.Ordered("D", "C"));
            Assert.IsFalse(route.Ordered("D", "D"));
        }

        [Test]
        public void ListContainsReversedRoute()
        {
            var list = new List<Route<int>>();
            var route = new Route<int>(5, 0xbad, 0xf00d);
            list.Add(route);
            var reverse = ~route;
            Assert.IsTrue(list.Contains(route));
            Assert.IsTrue(list.Contains(reverse));
        }

        [Test]
        public void ListRemoveReversedRoute()
        {
            var list = new List<Route<int>>();
            var route = new Route<int>(5, 0xbad, 0xf00d);
            list.Add(route);
            var reverse = ~route;
            list.Remove(reverse);
            Assert.AreEqual(0, list.Count);
        }

        [Test]
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

        [Test]
        public void ConcatShortRoutes()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xdead);
            var routeC = ~routeA;
            var routeD = ~routeB;

            var a = routeA.Join(routeB, false);
            var b = routeA.Join(routeD, false);
            var c = routeB.Join(routeA, false);
            var d = routeB.Join(routeC, false);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreEqual(b, c);
            Assert.AreEqual(b, d);
            Assert.AreEqual(c, d);
        }

        [Test]
        public void CanConcatDirected1()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xdead);

            Assert.IsFalse(routeA.CanConnectTo(routeA, true));
            Assert.IsTrue(routeA.CanConnectTo(routeB, true));
            Assert.IsFalse(routeB.CanConnectTo(routeA, true));
            Assert.IsFalse(routeB.CanConnectTo(routeB, true));
        }

        [Test]
        public void CanConcatDirected2()
        {
            var routeA = new Route<string>(1, "A", "B", "C");
            var routeB = new Route<string>(1, "C", "D", "F");
            var routeC = new Route<string>(1, "B", "G");

            Assert.IsFalse(routeA.CanConnectTo(routeA, true));
            Assert.IsTrue(routeA.CanConnectTo(routeB, true));
            Assert.IsFalse(routeB.CanConnectTo(routeA, true));
            Assert.IsFalse(routeB.CanConnectTo(routeB, true));
            Assert.IsFalse(routeA.CanConnectTo(routeC, true));
            Assert.IsFalse(routeC.CanConnectTo(routeA, true));
        }

        [Test]
        public void ConcatMediumRoutes()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead);
            var routeB = new Route<int>(1, 0xdead, 0xf00f, 0x1337);
            var routeC = ~routeA;
            var routeD = ~routeB;

            var a = routeA.Join(routeB, false);
            var b = routeA.Join(routeD, false);
            var c = routeB.Join(routeA, false);
            var d = routeB.Join(routeC, false);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreEqual(b, c);
            Assert.AreEqual(b, d);
            Assert.AreEqual(c, d);
        }

        [Test]
        public void ConcatLongRoutes()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead, 0xf00f);
            var routeB = new Route<int>(1, 0xf00f, 0x1337, 0x4444, 0xaaaa);
            var routeC = ~routeA;
            var routeD = ~routeB;

            var a = routeA.Join(routeB, false);
            var b = routeA.Join(routeD, false);
            var c = routeB.Join(routeA, false);
            var d = routeB.Join(routeC, false);

            Assert.AreEqual(a, b);
            Assert.AreEqual(a, c);
            Assert.AreEqual(a, d);
            Assert.AreEqual(b, c);
            Assert.AreEqual(b, d);
            Assert.AreEqual(c, d);
        }

        [Test]
        public void SinglePointIntersect()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead);
            var routeB = new Route<int>(1, 0xdead, 0xbeef, 0x1337);
            Assert.IsTrue(routeA.Intersects(routeB));
            Assert.IsTrue(routeB.Intersects(routeA));
        }

        [Test]
        public void TwoPointIntersect()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xdead, 0xf00f);
            var routeB = new Route<int>(1, 0xf00f, 0xbeef, 0xdead, 0xaaaa);
            Assert.IsTrue(routeA.Intersects(routeB));
            Assert.IsTrue(routeB.Intersects(routeA));
        }

        [Test]
        public void ConnectionContainsConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbad, 0xbeef);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ReverseConnectionContainsConnection()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbad, 0xbeef);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionContainsReverseConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xbad, 0xbeef);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionDoesNotContainConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [Test]
        public void ReverseConnectionDoesNotContainConnection()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionDoesNotContainReverseConnection()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionContainsPath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ReverseConnectionContainsPath3()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionContainsReversePath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xf00f, 0xbad, 0xbeef);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionContainsPath4()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef, 0x1337);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ReverseConnectionContainsPath4()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef);
            var routeB = new Route<int>(1, 0xf00f, 0xbad, 0xbeef, 0x1337);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ConnectionContainsReversePath4()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef);
            var routeB = ~new Route<int>(1, 0xf00f, 0xbad, 0xbeef, 0x1337);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void Path3ContainsPath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void ReversePath3ContainsPath3()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void Path3ContainsReversePath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            var routeB = ~new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsTrue(routeA.Contains(routeB));
            Assert.IsTrue(routeB.Contains(routeA));
        }

        [Test]
        public void Path3DoesntContainPath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf003);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [Test]
        public void ReversePath3DoesntContainPath3()
        {
            var routeA = ~new Route<int>(1, 0xbad, 0xbeef, 0xf003);
            var routeB = new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }

        [Test]
        public void Path3DoesntContainReversePath3()
        {
            var routeA = new Route<int>(1, 0xbad, 0xbeef, 0xf003);
            var routeB = ~new Route<int>(1, 0xbad, 0xbeef, 0xf00d);
            Assert.IsFalse(routeA.Contains(routeB));
            Assert.IsFalse(routeB.Contains(routeA));
        }
    }
}
