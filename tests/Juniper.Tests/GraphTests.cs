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
        [TestMethod]
        public void OneConnection()
        {
            var graph = new Graph<string>();
            graph.Connect("a", "b", 1);
            graph.AddEndPoint("b");
            graph.Solve();

            var route = graph.GetRoute("a", "b");
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
            Assert.AreEqual(1, route.Cost);
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
            Assert.AreEqual(2, route.Cost);
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

            var routeA = graph.GetRoute(start, end);

            var text = json.ToString(graph);
            graph = json.Parse(text);

            var routeB = graph.GetRoute(start, end);

            Assert.AreEqual(routeA.Start, routeB.Start);
            Assert.AreEqual(routeA.End, routeB.End);
            Assert.AreEqual(routeA.Cost, routeB.Cost);
            Assert.AreEqual(routeA.Count, routeB.Count);
            Assert.AreEqual(routeA.ToString(), routeB.ToString());
        }

        [TestMethod]
        public void RealTest()
        {
            BuildFullMap(out var graph);

            var startPoint = graph.GetNamedEndPoint("Ancestor Hall");
            var endPoint = graph.GetNamedEndPoint("Tea Shop");
            Assert.IsTrue(graph.Exists(startPoint, endPoint));

            var route = graph.GetRoute(startPoint, endPoint);
            Assert.IsNotNull(route);
        }

        [TestMethod]
        public void DeserializationIsFasterThanComputing()
        {
            var json = new JsonFactory<Graph<string>>();

            var start = DateTime.Now;
            BuildFullMap(out var graph);
            var timeSpent1 = DateTime.Now - start;

            var text = json.ToString(graph);

            start = DateTime.Now;
            _ = json.Parse(text);
            var timeSpent2 = DateTime.Now - start;

            Assert.IsTrue(timeSpent1 > timeSpent2);
        }

        private static void BuildFullMap(out Graph<string> graph)
        {
            var userHome = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            var filesRoute = Path.Combine(userHome, "Projects", "Yarrow", "shared", "StreamingAssets");
            var files = Directory.GetFiles(filesRoute, "*.json");
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

            graph = new Graph<string>();
            foreach (var start in metadata.Values)
            {
                graph.AddEndPoint(start.pano_id);
                if (start.label != null)
                {
                    graph.SetEndPointName(start.pano_id, start.label);
                }

                foreach (var endID in start.navPoints)
                {
                    var end = metadata[endID];
                    var distance = start.location.Distance(end.location);
                    graph.Connect(start.pano_id, end.pano_id, distance);
                }
            }

            graph.Solve();
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
