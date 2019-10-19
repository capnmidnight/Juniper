using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Juniper.IO;
using Juniper.Units;
using Juniper.World.GIS;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.Collections.Tests
{
    [TestClass]
    public class GraphTests
    {
        private static float One<T>(T a, T b)
        {
            return 1;
        }

        [TestMethod]
        public void OneConnection()
        {
            var graph = new Graph<string>(One);
            graph.Connect("a", "b");
            graph.AddEndPoint("b");
            graph.Solve();

            var path = graph["a", "b"];
            Assert.AreEqual(1, path.Cost);
        }

        [TestMethod]
        public void TwoConnection()
        {
            var graph = new Graph<string>(One);
            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.AddEndPoint("c");
            graph.Solve();

            var path = graph["a", "c"];
            Assert.AreEqual(2, path.Cost);
        }

        [TestMethod]
        public void ReversedTwoConnection()
        {
            var graph = new Graph<string>(One);
            graph.Connect("b", "c");
            graph.Connect("a", "b");
            graph.AddEndPoint("c");
            graph.Solve();

            var path = graph["a", "c"];
            Assert.AreEqual(2, path.Cost);
        }

        [TestMethod]
        public void Reversed10Connection()
        {
            var graph = new Graph<string>(One);
            graph.Connect("j", "k");
            graph.Connect("i", "j");
            graph.Connect("h", "i");
            graph.Connect("g", "h");
            graph.Connect("f", "g");
            graph.Connect("e", "f");
            graph.Connect("d", "e");
            graph.Connect("c", "d");
            graph.Connect("b", "c");
            graph.Connect("a", "b");
            graph.AddEndPoint("k");
            graph.Solve();

            var path = graph["a", "k"];
            Assert.AreEqual(10, path.Cost);
        }

        [TestMethod]
        public void Disconnected()
        {
            var graph = new Graph<string>(One);
            graph.Connect("a", "b");
            graph.Connect("c", "d");
            graph.AddEndPoint("c");
            graph.Solve();

            var path = graph["a", "c"];
            Assert.IsNull(path);
        }

        [TestMethod]
        public void MiddleConnection()
        {
            var graph = new Graph<string>(One);
            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.AddEndPoint("d");
            graph.Solve();

            var path = graph["a", "d"];
            Assert.AreEqual(3, path.Cost);
        }

        [TestMethod]
        public void ShortcutConnection()
        {
            var graph = new Graph<string>(One);
            graph.Connect("a", "b");
            graph.Connect("b", "c");
            graph.Connect("c", "d");
            graph.Connect("a", "d");
            graph.AddEndPoint("d");
            graph.Solve();

            var path = graph["a", "d"];
            Assert.AreEqual(1, path.Cost);
        }

        [TestMethod]
        public void FullyConnected100Nodes()
        {
            var random = new Random();
            var graph = new Graph<int>((_, __) => random.Next(1, 101));

            var start = DateTime.Now;
            for (int a = 0; a < 100; ++a)
            {
                for (int b = 0; b < 100; ++b)
                {
                    if (a != b)
                    {
                        graph.Connect(a, b);
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
            var graph = new Graph<int>(One);
            var start = 7216;
            var end = 5666;
            graph.Connect(start, 4673);
            graph.Connect(4673, 3416);
            graph.Connect(4673, 4756);
            graph.Connect(4673, 9713);
            graph.Connect(4756, 1371);
            graph.Connect(9713, 1371);
            graph.Connect(1371, 3464);
            graph.Connect(3464, 2656);
            graph.Connect(3464, end);
            graph.AddEndPoint(end);
            graph.Solve();

            var pathA = graph[start, end];

            graph.Connect(4673, 1371);
            graph.Solve();

            var pathB = graph[start, end];
            Assert.IsTrue(pathB.Cost < pathA.Cost);
        }

        [TestMethod]
        public void ClosedLoop()
        {
            var graph = new Graph<int>(One);
            graph.Connect(0, 1);
            graph.Connect(1, 2);
            graph.Connect(2, 3);
            graph.Connect(3, 0);
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
            var graph = new Graph<int>((a, b) =>
            {
                if (a == 0 && b == 1)
                {
                    return 100;
                }
                else
                {
                    return 1;
                }
            });
            graph.Connect(0, 1);
            graph.Connect(0, 2);
            graph.Connect(2, 1);
            graph.AddEndPoint(1);
            graph.Solve();

            var path = graph[0, 1];
            Assert.AreEqual(2, path.Cost);
        }

        [TestMethod]
        public void Serialization()
        {
            var json = new JsonFactory<Graph<int>>();
            var graph = new Graph<int>(One);
            var start = 7216;
            var end = 5666;
            graph.Connect(start, 4673);
            graph.Connect(4673, 3416);
            graph.Connect(4673, 4756);
            graph.Connect(4673, 9713);
            graph.Connect(4756, 1371);
            graph.Connect(9713, 1371);
            graph.Connect(1371, 3464);
            graph.Connect(3464, 2656);
            graph.Connect(3464, end);
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

        [TestMethod]
        public void RealTest()
        {
            var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var filesPath = Path.Combine(userHome, "Projects", "Yarrow", "shared", "StreamingAssets");
            var files = Directory.GetFiles(filesPath, "*.json");
            var json = new JsonFactory<Metadata>();
            var metadata = files
                .Select(f =>
                {
                    if (json.TryDeserialize(f, out var datum))
                    {
                        return datum;
                    }
                    else
                    {
                        return null;
                    }
                })
                .Where(x => x?.pano_id != null)
                .ToDictionary(x => x.pano_id);

            var graph = new Graph<string>((a, b) =>
                metadata[a].location.Distance(metadata[b].location));

            var points = new Dictionary<string, string>();
            foreach (var start in metadata.Values)
            {
                if (start.label != null)
                {
                    graph.AddEndPoint(start.pano_id);
                    points.Add(start.label, start.pano_id);
                }

                foreach (var endID in start.navPoints)
                {
                    var end = metadata[endID];
                    var distance = start.location.Distance(end.location);
                    graph.Connect(start.pano_id, end.pano_id);
                }
            }

            graph.Solve();

            var startPoint = points["Ancestor Hall"];
            var endPoint = points["Tea Shop"];
            Assert.IsTrue(graph.Exists(startPoint, endPoint));

            var path = graph[startPoint, endPoint];
            Assert.IsNotNull(path);
        }

        private class Metadata
        {
            public string pano_id;
            public string label;
            public LatLngPoint location;
            public string[] navPoints;
        }
    }
}
