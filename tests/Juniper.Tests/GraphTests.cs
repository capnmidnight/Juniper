using System;
using Juniper.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{
    [TestClass]
    public class GraphTests
    {
        [TestMethod]
        public void OneConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.AddEndPoint("b");
            graph.Solve();

            var path = graph["a", "b"];
            Assert.AreEqual(1, path.Cost);
        }

        [TestMethod]
        public void TwoConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.AddEndPoint("c");
            graph.Solve();

            var path = graph["a", "c"];
            Assert.AreEqual(2, path.Cost);
        }

        [TestMethod]
        public void ReversedTwoConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("b", "c", 1);
            graph.Connect("a", "b", 1);
            graph.AddEndPoint("c");
            graph.Solve();

            var path = graph["a", "c"];
            Assert.AreEqual(2, path.Cost);
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

            var path = graph["a", "k"];
            Assert.AreEqual(10, path.Cost);
        }

        [TestMethod]
        public void Disconnected()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("c", "d", 1);
            graph.AddEndPoint("c");
            graph.Solve();

            var path = graph["a", "c"];
            Assert.IsNull(path);
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

            var path = graph["a", "d"];
            Assert.AreEqual(3, path.Cost);
        }

        [TestMethod]
        public void ShortcutConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.Connect("b", "c", 1);
            graph.Connect("c", "d", 1);
            graph.Connect("a", "d", 1);
            graph.AddEndPoint("d");
            graph.Solve();

            var path = graph["a", "d"];
            Assert.AreEqual(1, path.Cost);
        }

        [TestMethod]
        public void FullyConnected100Nodes()
        {
            var graph = new Graph<int>();
            var random = new Random();

            var start = DateTime.Now;
            for (int a = 0; a < 100; ++a)
            {
                for (int b = 0; b < 100; ++b)
                {
                    if (a != b)
                    {
                        graph.Connect(a, b, random.Next(1, 101));
                        graph.AddEndPoint(b);
                    }
                }
            }

            graph.Solve();

            var delta = DateTime.Now - start;
            Assert.IsTrue(delta.TotalSeconds < 3);
        }

        [TestMethod]
        public void InterestingGraph()
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

            var pathA = graph[start, end];

            graph.Connect(4673, 1371, (float)Math.Sqrt(2));
            graph.Solve();

            var pathB = graph[start, end];
            Assert.IsTrue(pathB.Cost < pathA.Cost);
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

            var pathA = graph[0, 1];
            var pathB = graph[0, 2];
            var pathC = graph[0, 3];

            Assert.AreEqual(1, pathA.Cost);
            Assert.AreEqual(2, pathB.Cost);
            Assert.AreEqual(1, pathC.Cost);
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

            var path = graph[0, 1];
            Assert.AreEqual(2, path.Cost);
        }

        [TestMethod]
        public void Serialization()
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

            var pathA = graph[start, end];

            var text = json.ToString(graph);
            graph = json.Parse(text);

            var pathB = graph[start, end];

            Assert.AreEqual(pathA.Start, pathB.Start);
            Assert.AreEqual(pathA.End, pathB.End);
            Assert.AreEqual(pathA.Cost, pathB.Cost);
            Assert.AreEqual(pathA.Count, pathB.Count);
            Assert.AreEqual(pathA.ToString(), pathB.ToString());
        }
    }
}
