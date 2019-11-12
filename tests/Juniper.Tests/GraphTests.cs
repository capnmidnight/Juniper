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
        private static void CheckGraph(string label, Graph<string> graph1, Graph<string> graph2)
        {
            foreach (var node in graph1.Nodes)
            {
                Assert.IsTrue(graph2.NodeExists(node), $"{label}: Node {node} does not exist");

                foreach (var route in graph1.GetRoutes(node))
                {
                    Assert.IsTrue(graph2.RouteExists(route.Start, route.End), $"{label}: Route {route.Start} => {route.End} does not exist");
                }
            }
        }

        private static void CheckGraphs(string label, Graph<string> graph1, Graph<string> graph2)
        {
            CheckGraph(label + " A->B", graph1, graph2);
            CheckGraph(label + " B->A", graph2, graph1);
        }

        [TestMethod]
        public void OneConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Solve();

            var route = graph.GetRoute("a", "b");
            var connectsA = graph.GetExits("a");
            var connectsB = graph.GetExits("b");

            Assert.IsNotNull(route);
            Assert.AreEqual(1, route.Cost);
            Assert.IsTrue(connectsA.Contains("b"));
            Assert.IsTrue(connectsB.Contains("a"));
            Assert.IsFalse(connectsA.Contains("a"));
            Assert.IsFalse(connectsB.Contains("b"));
        }

        [TestMethod]
        public void TwoConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Solve();

            var route = graph.GetRoute("a", "c");
            var connectsB = graph.GetExits("b");

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
            Assert.IsTrue(connectsB.Contains("a"));
            Assert.IsTrue(connectsB.Contains("c"));
        }

        [TestMethod]
        public void ReversedTwoConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("b", "c", 1);
            graph.Connect("a", "b", 1);
            graph.Solve();

            var route = graph.GetRoute("a", "c");
            var connectsB = graph.GetExits("b");

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
            Assert.IsTrue(connectsB.Contains("a"));
            Assert.IsTrue(connectsB.Contains("c"));
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
            graph.Solve();

            var route = graph.GetRoute("a", "d");

            Assert.IsNull(route);
        }

        [TestMethod]
        public void MiddleConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("c", "d", 1);
            graph.Solve();

            var routeA = graph.GetRoute("a", "d");
            var routeB = graph.GetRoute("d", "a");
            var paths = graph.GetRoutes("d");

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreEqual(3, routeA.Cost);
            Assert.AreEqual(routeA, routeB);
            Assert.AreEqual(3, paths.Count());
        }

        [TestMethod]
        public void Shortcut()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("c", "d", 1);
            graph.Connect("a", "d", 1);
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

            graph.Solve();
            var routeA = graph.GetRoute("a", "d");

            graph.Disconnect("a", "d");
            graph.Solve();
            var routeB = graph.GetRoute("a", "d");

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreNotEqual(routeA, routeB);
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
            graph.Solve();

            var routeA = graph.GetRoute(start, end);

            graph.Connect(4673, 1371, 1);
            graph.Solve();

            var routeB = graph.GetRoute(start, end);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreNotEqual(routeA, routeB);
            Assert.IsTrue(routeA.Cost > routeB.Cost);
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
            graph.Solve();

            var routeA = graph.GetRoute(start, end);
            Assert.IsNotNull(routeA);

            graph.Connect(1371, 3464, 2);
            graph.Solve();

            var routeB = graph.GetRoute(start, end);

            Assert.IsNotNull(routeB);
            Assert.AreNotEqual(routeA, routeB);
            Assert.IsTrue(routeA.Cost < routeB.Cost);
        }

        [TestMethod]
        public void ClosedLoop()
        {
            var graph = new Graph<int>();
            graph.Connect(0, 1, 1);
            graph.Connect(1, 2, 1);
            graph.Connect(2, 3, 1);
            graph.Connect(3, 0, 1);
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
            var file = Path.Combine("..", "..", "..", "test.json");
            var text = File.ReadAllText(file);
            Assert.IsTrue(json.TryParse(text, out var graph));
            graph.Solve();
            Assert.IsTrue(graph.Nodes.Count() > 0);
        }

        [TestMethod]
        public void BinaryDeserialization()
        {
            var json = new BinaryFactory<Graph<string>>();
            var file = Path.Combine("..", "..", "..", "test.bin");
            var bytes = File.ReadAllBytes(file);
            Assert.IsTrue(json.TryDeserialize(bytes, out var graph));
            graph.Solve();
            Assert.IsTrue(graph.Nodes.Count() > 0);
        }

        [TestMethod]
        public void SimpleClone()
        {
            var graph1 = new Graph<string>();
            graph1.Connect("a", "b", 1);
            graph1.Connect("b", "c", 1);
            graph1.Connect("c", "d", 1);
            graph1.Connect("a", "d", 1);
            graph1.Solve();

            var graph2 = graph1.Clone();
            graph2.Solve();

            CheckGraphs("Clone", graph1, graph2);
        }

        [TestMethod]
        public void FullClone()
        {
            var graph1 = new Graph<string>();
            graph1.Connect("a", "b", 1);
            graph1.Connect("b", "c", 1);
            graph1.Connect("c", "d", 1);
            graph1.Connect("a", "d", 1);
            graph1.Solve();

            graph1.SaveAllPaths = true;
            var graph2 = graph1.Clone();

            CheckGraphs("Clone", graph1, graph2);
        }

        private static void DeserializationTest(string ext)
        {
            var graph1 = Graph<string>.Load(Path.Combine("..", "..", "..", "test." + ext));
            graph1.Solve();

            var graph2 = Graph<string>.Load(Path.Combine("..", "..", "..", "test2." + ext));

            CheckGraphs("Bin", graph1, graph2);
        }

        [TestMethod]
        public void JsonDeserializationExactlyDuplicatesGraph()
        {
            DeserializationTest("json");
        }

        [TestMethod]
        public void BinaryDeserializationExactlyDuplicatesGraph()
        {
            DeserializationTest("bin");
        }
    }
}
