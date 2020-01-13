using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;

using Juniper.IO;

using NUnit.Framework;

namespace Juniper.Collections.Tests
{
    [TestFixture]
    public class GraphTests
    {
        private static void CheckGraphs(string label, Graph<string> graph1, Graph<string> graph2)
        {
            CheckGraph(label + " A->B", graph1, graph2);
            CheckGraph(label + " B->A", graph2, graph1);
        }

        private static void CheckGraph(string label, Graph<string> graph1, Graph<string> graph2)
        {
            foreach (var node in graph1.Nodes)
            {
                Assert.IsTrue(graph2.Exists(node), $"{label}: Node {node} does not exist");

                foreach (var route in graph1.FindRoutes(node))
                {
                    Assert.IsTrue(graph2.Exists(route.Start, route.End), $"{label}: Route {route.Start} => {route.End} does not exist");
                }
            }
        }

        [Test]
        public void OneConnection()
        {
            var graph = new Graph<string>();
            graph.SetConnection("a", "b", 1);
            graph.Solve();

            var route = graph.FindRoute("a", "b");
            var connectsA = graph.FindExits("a");
            var connectsB = graph.FindExits("b");

            Assert.IsNotNull(route);
            Assert.AreEqual(1, route.Cost);
            Assert.IsTrue(connectsA.Contains("b"));
            Assert.IsTrue(connectsB.Contains("a"));
            Assert.IsFalse(connectsA.Contains("a"));
            Assert.IsFalse(connectsB.Contains("b"));
        }

        [Test]
        public void TwoConnection()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("a", "b", 1),
                ("b", "c", 1));
            graph.Solve();

            var route = graph.FindRoute("a", "c");
            var connectsB = graph.FindExits("b");

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
            Assert.IsTrue(connectsB.Contains("a"));
            Assert.IsTrue(connectsB.Contains("c"));
        }

        [Test]
        public void ReversedTwoConnection()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("b", "c", 1),
                ("a", "b", 1));
            graph.Solve();

            var route = graph.FindRoute("a", "c");
            var connectsB = graph.FindExits("b");

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
            Assert.IsTrue(connectsB.Contains("a"));
            Assert.IsTrue(connectsB.Contains("c"));
        }

        [Test]
        public void Reversed10Connection()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("j", "k", 1),
                ("i", "j", 1),
                ("h", "i", 1),
                ("g", "h", 1),
                ("f", "g", 1),
                ("e", "f", 1),
                ("d", "e", 1),
                ("c", "d", 1),
                ("b", "c", 1),
                ("a", "b", 1));
            graph.Solve();

            var route = graph.FindRoute("a", "k");

            Assert.IsNotNull(route);
            Assert.AreEqual(10, route.Cost);
        }

        [Test]
        public void Disconnected()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("a", "b", 1),
                ("c", "d", 1));
            graph.Solve();

            var route = graph.FindRoute("a", "d");

            Assert.IsNull(route);
        }

        [Test]
        public void MiddleConnection()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("a", "b", 1),
                ("b", "c", 1),
                ("c", "d", 1));
            graph.Solve();

            var routeA = graph.FindRoute("a", "d");
            var routeB = graph.FindRoute("d", "a");
            var paths = graph.FindRoutes("d");

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreEqual(3, routeA.Cost);
            Assert.AreEqual(routeA, routeB);
            Assert.AreEqual(3, paths.Count());
        }

        [Test]
        public void Shortcut()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("a", "b", 1),
                ("b", "c", 1),
                ("c", "d", 1),
                ("a", "d", 1));
            graph.Solve();

            var route = graph.FindRoute("a", "d");

            Assert.IsNotNull(route);
            Assert.AreEqual(1, route.Cost);
        }

        [Test]
        public void RemoveShortcut()
        {
            var graph = new Graph<string>();
            graph.SetConnections(
                ("a", "b", 1),
                ("b", "c", 1),
                ("c", "d", 1),
                ("a", "d", 1));

            graph.Solve();
            var routeA = graph.FindRoute("a", "d");

            graph.Remove("a", "d");
            graph.Solve();
            var routeB = graph.FindRoute("a", "d");

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreNotEqual(routeA, routeB);
        }

        [Test]
        public void AddShortcut()
        {
            var graph = new Graph<int>();
            const int start = 7216;
            const int end = 5666;
            graph.SetConnections(
                (start, 4673, 1),
                (4673, 3416, 1),
                (4673, 4756, 1),
                (4673, 9713, 1),
                (4756, 1371, 1),
                (9713, 1371, 1),
                (1371, 3464, 1),
                (3464, 2656, 1),
                (3464, end, 1));
            graph.Solve();

            var routeA = graph.FindRoute(start, end);

            graph.SetConnection(4673, 1371, 1);
            graph.Solve();

            var routeB = graph.FindRoute(start, end);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreNotEqual(routeA, routeB);
            Assert.IsTrue(routeA.Cost > routeB.Cost);
        }

        [Test]
        public void AddTraffic()
        {
            var graph = new Graph<int>();
            const int start = 7216;
            const int end = 5666;
            graph.SetConnections(
                (start, 4673, 1),
                (4673, 3416, 1),
                (4673, 4756, 1),
                (4673, 9713, 1),
                (4756, 1371, 1),
                (9713, 1371, 1),
                (1371, 3464, 1),
                (3464, 2656, 1),
                (3464, end, 1));
            graph.Solve();

            var routeA = graph.FindRoute(start, end);
            Assert.IsNotNull(routeA);

            graph.SetConnection(1371, 3464, 2);
            graph.Solve();

            var routeB = graph.FindRoute(start, end);

            Assert.IsNotNull(routeB);
            Assert.AreNotEqual(routeA, routeB);
            Assert.IsTrue(routeA.Cost < routeB.Cost);
        }

        [Test]
        public void ClosedLoop()
        {
            var graph = new Graph<int>();
            graph.SetConnections(
                (0, 1, 1),
                (1, 2, 1),
                (2, 3, 1),
                (3, 0, 1));
            graph.Solve();

            var routeA = graph.FindRoute(0, 1);
            var routeB = graph.FindRoute(0, 2);
            var routeC = graph.FindRoute(0, 3);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.IsNotNull(routeC);
            Assert.AreEqual(1, routeA.Cost);
            Assert.AreEqual(2, routeB.Cost);
            Assert.AreEqual(1, routeC.Cost);
        }

        [Test]
        public void DetourExpensiveRoute()
        {
            var graph = new Graph<int>();
            graph.SetConnections(
                (0, 1, 100),
                (0, 2, 1),
                (2, 1, 1));
            graph.Solve();

            var route = graph.FindRoute(0, 1);

            Assert.IsNotNull(route);
            Assert.AreEqual(2, route.Cost);
        }

        [Test]
        public void Clone()
        {
            var graph1 = new Graph<string>();
            graph1.SetConnections(
                ("a", "b", 1),
                ("b", "c", 1),
                ("c", "d", 1),
                ("a", "d", 1));
            graph1.Solve();

            var graph2 = graph1.Clone();

            CheckGraphs("Clone", graph1, graph2);
        }

        private static void SerializationTest<FactoryT>()
            where FactoryT : IFactory<Graph<int>, MediaType.Application>, new()
        {
            var factory = new FactoryT();
            var graph = new Graph<int>();
            const int start = 7216;
            const int end = 5666;
            graph.SetConnections(
                (start, 4673, 1),
                (4673, 3416, 1),
                (4673, 4756, 1),
                (4673, 9713, 1),
                (4756, 1371, 1),
                (9713, 1371, 1),
                (1371, 3464, 1),
                (3464, 2656, 1),
                (3464, end, 1));
            graph.Solve();

            var routeA = graph.FindRoute(start, end);

            var bytes = factory.Serialize(graph);
            graph = factory.Deserialize(bytes);
            graph.Solve();

            var routeB = graph.FindRoute(start, end);

            Assert.IsNotNull(routeA);
            Assert.IsNotNull(routeB);
            Assert.AreEqual(routeA.Start, routeB.Start);
            Assert.AreEqual(routeA.End, routeB.End);
            Assert.AreEqual(routeA.Cost, routeB.Cost);
            Assert.AreEqual(routeA.Count, routeB.Count);
            Assert.AreEqual(routeA.ToString(), routeB.ToString());
        }

        [Test]
        public void SerializationJson()
        {
            SerializationTest<JsonFactory<Graph<int>>>();
        }

        [Test]
        public void SerializationBinary()
        {
            SerializationTest<BinaryFactory<Graph<int>>>();
        }

        private static void DeserializationTest<FactoryT>(string ext)
            where FactoryT : IFactory<Graph<string>, MediaType.Application>, new()
        {
            var factory = new FactoryT();
            var file = "test." + ext;
            var bytes = File.ReadAllBytes(file);
            Assert.IsTrue(factory.TryDeserialize(bytes, out var graph));
            graph.Solve();
            Assert.IsTrue(graph.Nodes.Any());
        }

        [Test]
        public void DeserializationJson()
        {
            DeserializationTest<JsonFactory<Graph<string>>>("json");
        }

        [Test]
        public void DeserializationBinary()
        {
            DeserializationTest<BinaryFactory<Graph<string>>>("bin");
        }

        private static void DeserializationTest(string ext)
        {
            var inPath = "test." + ext;
            var outPath = "test2." + ext;

            var stopwatch = new Stopwatch();

            stopwatch.Start();
            var graph1 = Graph.Load<string>(inPath);
            graph1.Solve();
            stopwatch.Stop();
            var time1 = stopwatch.Elapsed;

            graph1.Save(outPath);

            stopwatch.Restart();
            var graph2 = Graph.Load<string>(outPath);
            stopwatch.Stop();
            var time2 = stopwatch.Elapsed;

            CheckGraphs("Bin", graph1, graph2);

            Assert.IsTrue(time2 < time1, $"{time2.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)} > {time1.TotalMilliseconds.ToString(CultureInfo.CurrentCulture)}");
        }

        [Test]
        public void DeserializationJsonExactlyDuplicatesGraph()
        {
            DeserializationTest("json");
        }

        [Test]
        public void DeserializationBinaryExactlyDuplicatesGraph()
        {
            DeserializationTest("bin");
        }
    }
}
