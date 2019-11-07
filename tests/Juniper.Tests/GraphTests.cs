using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Juniper.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void ListContainsReversedRoute()
        {
            var list = new List<Route<int>>();
            var route = new Route<int>(0xbad, 0xf00d, 5);
            list.Add(route);
            var reverse = ~route;
            Assert.IsTrue(list.Contains(route));
            Assert.IsTrue(list.Contains(reverse));
        }

        [TestMethod]
        public void ListRemoveReversedRoute()
        {
            var list = new List<Route<int>>();
            var route = new Route<int>(0xbad, 0xf00d, 5);
            list.Add(route);
            var reverse = ~route;
            list.Remove(reverse);
            Assert.AreEqual(0, list.Count);
        }

        [TestMethod]
        public void DistinctRoutes()
        {
            var route = new Route<int>(0xbad, 0xf00d, 5);
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
            var routeA = new Route<int>(0xbad, 0xbeef, 1);
            var routeB = new Route<int>(0xbeef, 0xdead, 1);
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
            var routeA = new Route<int>(0xbad, 0xbeef, 1)
                + new Route<int>(0xbeef, 0xdead, 1);
            var routeB = new Route<int>(0xdead, 0xf00f, 1)
                + new Route<int>(0xf00f, 0x1337, 1);
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
            var routeA = new Route<int>(0xbad, 0xbeef, 1)
                + new Route<int>(0xbeef, 0xdead, 1)
                + new Route<int>(0xdead, 0xf00f, 1);
            var routeB = new Route<int>(0xf00f, 0x1337, 1)
                + new Route<int>(0x1337, 0x4444, 1)
                + new Route<int>(0x4444, 0xaaaa, 1);
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
        public void SinglePointOverlap()
        {
            var routeA = new Route<int>(0xbad, 0xbeef, 1)
                + new Route<int>(0xbeef, 0xdead, 1);
            var routeB = new Route<int>(0xdead, 0xbeef, 1)
                + new Route<int>(0xbeef, 0x1337, 1);
            Assert.IsTrue(routeA.Overlaps(routeB));
            Assert.IsTrue(routeB.Overlaps(routeA));
        }

        [TestMethod]
        public void TwoPointOverlap()
        {
            var routeA = new Route<int>(0xbad, 0xbeef, 1)
                + new Route<int>(0xbeef, 0xdead, 1)
                + new Route<int>(0xdead, 0xf00f, 1);
            var routeB = new Route<int>(0xf00f, 0xbeef, 1)
                + new Route<int>(0xbeef, 0xdead, 1)
                + new Route<int>(0xdead, 0xaaaa, 1);
            Assert.IsTrue(routeA.Overlaps(routeB));
            Assert.IsTrue(routeB.Overlaps(routeA));
        }

        [TestMethod]
        public void OneConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.AddEndPoint("b");
            graph.Solve();

            var route = graph.GetRoute("a", "b");

            Assert.IsNotNull(route);
            Assert.AreEqual(1, route.Cost);
        }

        [TestMethod]
        public void TwoConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.AddEndPoint("c");
            graph.Solve();

            var route = graph.GetRoute("a", "c");

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
        }

        [TestMethod]
        public void ReversedTwoConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("b", "c", 1);
            graph.Connect("a", "b", 1);
            graph.AddEndPoint("c");
            graph.Solve();

            var route = graph.GetRoute("a", "c");

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
        }

        [TestMethod]
        public void Reversed10Connection()
        {
            var graph = new Graph<string>();
            graph.Connect("j", "k", 1);
            graph.Connect("i", "j", 1);
            graph.Connect("h", "i", 1);
            graph.Connect("g", "h", 1);
            graph.Connect("f", "g", 1);
            graph.Connect("e", "f", 1);
            graph.Connect("d", "e", 1);
            graph.Connect("c", "d", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("a", "b", 1);
            graph.AddEndPoint("k");
            graph.Solve();

            var route = graph.GetRoute("a", "k");

            Assert.IsNotNull(route);
            Assert.AreEqual(10, route.Cost);
        }

        [TestMethod]
        public void Disconnected()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("c", "d", 1);
            graph.AddEndPoint("c");
            graph.Solve();

            var route = graph.GetRoute("a", "c");

            Assert.IsNull(route);
        }

        [TestMethod]
        public void MiddleConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("c", "d", 1);
            graph.AddEndPoint("d");
            graph.Solve();

            var route = graph.GetRoute("a", "d");

            Assert.IsNotNull(route);
            Assert.AreEqual(3, route.Cost);
        }

        [TestMethod]
        public void Shortcut()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("c", "d", 1);
            graph.Connect("a", "d", 1);
            graph.AddEndPoint("d");
            graph.Solve();

            var route = graph.GetRoute("a", "d");

            Assert.IsNotNull(route);
            Assert.AreEqual(1, route.Cost);
        }

        [TestMethod]
        public void RemoveShortcut()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("c", "d", 1);
            graph.Connect("a", "d", 1);
            graph.AddEndPoint("d");

            graph.Solve();
            var routeA = graph.GetRoute("a", "d");

            graph.Disconnect("a", "d");
            graph.Solve();
            var routeB = graph.GetRoute("a", "d");

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.IsTrue(routeA.Cost < routeB.Cost);
        }

        [TestMethod]
        public void AddShortcut()
        {
            var graph = new Graph<int>();
            var start = 7216;
            var end = 5666;
            graph.Connect(start, 4673, 1);
            graph.Connect(4673, 3416, 1);
            graph.Connect(4673, 4756, 1);
            graph.Connect(4673, 9713, 1);
            graph.Connect(4756, 1371, 1);
            graph.Connect(9713, 1371, 1);
            graph.Connect(1371, 3464, 1);
            graph.Connect(3464, 2656, 1);
            graph.Connect(3464, end, 1);
            graph.AddEndPoint(end);
            graph.Solve();

            var routeA = graph.GetRoute(start, end);

            graph.Connect(4673, 1371, 1);
            graph.Solve();

            var routeB = graph.GetRoute(start, end);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.IsTrue(routeB.Cost < routeA.Cost);
        }

        [TestMethod]
        public void AddTraffic()
        {
            var graph = new Graph<int>();
            var start = 7216;
            var end = 5666;
            graph.Connect(start, 4673, 1);
            graph.Connect(4673, 3416, 1);
            graph.Connect(4673, 4756, 1);
            graph.Connect(4673, 9713, 1);
            graph.Connect(4756, 1371, 1);
            graph.Connect(9713, 1371, 1);
            graph.Connect(1371, 3464, 1);
            graph.Connect(3464, 2656, 1);
            graph.Connect(3464, end, 1);
            graph.AddEndPoint(end);
            graph.Solve();

            var routeA = graph.GetRoute(start, end);
            graph.Connect(1371, 3464, 2);
            graph.Solve();

            var routeB = graph.GetRoute(start, end);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.IsTrue(routeB.Cost > routeA.Cost);
        }

        [TestMethod]
        public void ClosedLoop()
        {
            var graph = new Graph<int>();
            graph.Connect(0, 1, 1);
            graph.Connect(1, 2, 1);
            graph.Connect(2, 3, 1);
            graph.Connect(3, 0, 1);
            graph.AddEndPoint(1);
            graph.AddEndPoint(2);
            graph.AddEndPoint(3);
            graph.Solve();

            var routeA = graph.GetRoute(0, 1);
            var routeB = graph.GetRoute(0, 2);
            var routeC = graph.GetRoute(0, 3);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.IsNotNull(routeC);
            Assert.AreEqual(1, routeA.Cost);
            Assert.AreEqual(2, routeB.Cost);
            Assert.AreEqual(1, routeC.Cost);
        }

        [TestMethod]
        public void DetourExpensiveRoute()
        {
            var graph = new Graph<int>();
            graph.Connect(0, 1, 100);
            graph.Connect(0, 2, 1);
            graph.Connect(2, 1, 1);
            graph.AddEndPoint(1);
            graph.Solve();

            var route = graph.GetRoute(0, 1);

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
        }

        [TestMethod]
        public void JsonSerialization()
        {
            var json = new JsonFactory<Graph<int>>();
            var graph = new Graph<int>();
            var start = 7216;
            var end = 5666;
            graph.Connect(start, 4673, 1);
            graph.Connect(4673, 3416, 1);
            graph.Connect(4673, 4756, 1);
            graph.Connect(4673, 9713, 1);
            graph.Connect(4756, 1371, 1);
            graph.Connect(9713, 1371, 1);
            graph.Connect(1371, 3464, 1);
            graph.Connect(3464, 2656, 1);
            graph.Connect(3464, end, 1);
            graph.AddEndPoint(end);
            graph.Solve();

            var routeA = graph.GetRoute(start, end);

            var text = json.ToString(graph);
            graph = json.Parse(text);
            graph.Solve();

            var routeB = graph.GetRoute(start, end);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreEqual(routeA.Start, routeB.Start);
            Assert.AreEqual(routeA.End, routeB.End);
            Assert.AreEqual(routeA.Cost, routeB.Cost);
            Assert.AreEqual(routeA.Count, routeB.Count);
            Assert.AreEqual(routeA.ToString(), routeB.ToString());
        }

        [TestMethod]
        public void JsonDeserialization()
        {
            var json = new JsonFactory<Graph<string>>();
            var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var file = Path.Combine(userHome, "Projects", "Yarrow", "shared", "StreamingAssets", "paths.json");
            var text = File.ReadAllText(file);
            Assert.IsTrue(json.TryParse(text, out var graph));
            Assert.IsNotNull(graph);

            graph.Solve();
        }
    }
}
