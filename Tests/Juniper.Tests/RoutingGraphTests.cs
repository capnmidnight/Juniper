using Juniper.IO;

using NUnit.Framework;
using Assert = NUnit.Framework.Legacy.ClassicAssert;

using System;
using System.IO;
using System.Linq;

namespace Juniper.Collections;

[TestFixture]
public class RoutingGraphTests
{
    private static void CheckGraphs(string label, RoutingGraph<string> graph1, RoutingGraph<string> graph2)
    {
        CheckGraph(label + " A->B", graph1, graph2);
        CheckGraph(label + " B->A", graph2, graph1);
    }

    private static void CheckGraph(string label, RoutingGraph<string> graph1, RoutingGraph<string> graph2)
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
        var graph = new RoutingGraph<string>();
        graph.SetConnection("a", "b");
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
        var graph = new RoutingGraph<string>();
        graph.SetConnections(
            ("a", "b"),
            ("b", "c"));
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
        var graph = new RoutingGraph<string>();
        graph.SetConnections(
            ("b", "c"),
            ("a", "b"));
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
        var graph = new RoutingGraph<string>();
        graph.SetConnections(
            ("j", "k"),
            ("i", "j"),
            ("h", "i"),
            ("g", "h"),
            ("f", "g"),
            ("e", "f"),
            ("d", "e"),
            ("c", "d"),
            ("b", "c"),
            ("a", "b"));
        graph.Solve();

        var route = graph.FindRoute("a", "k");

        Assert.IsNotNull(route);
        Assert.AreEqual(10, route.Cost);
    }

    [Test]
    public void Disconnected()
    {
        var graph = new RoutingGraph<string>();
        graph.SetConnections(
            ("a", "b"),
            ("c", "d"));
        graph.Solve();

        var route = graph.FindRoute("a", "d");

        Assert.IsNull(route);
    }

    [Test]
    public void MiddleConnection()
    {
        var graph = new RoutingGraph <string>();
        graph.SetConnections(
            ("a", "b"),
            ("b", "c"),
            ("c", "d"));
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
        var graph = new RoutingGraph <string>();
        graph.SetConnections(
            ("a", "b"),
            ("b", "c"),
            ("c", "d"),
            ("a", "d"));
        graph.Solve();

        var route = graph.FindRoute("a", "d");

        Assert.IsNotNull(route);
        Assert.AreEqual(1, route.Cost);
    }

    [Test]
    public void RemoveShortcut()
    {
        var graph = new RoutingGraph <string>();
        graph.SetConnections(
            ("a", "b"),
            ("b", "c"),
            ("c", "d"),
            ("a", "d"));

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
        var graph = new RoutingGraph <int>();
        const int start = 7216;
        const int end = 5666;
        graph.SetConnections(
            (start, 4673),
            (4673, 3416),
            (4673, 4756),
            (4673, 9713),
            (4756, 1371),
            (9713, 1371),
            (1371, 3464),
            (3464, 2656),
            (3464, end));
        graph.Solve();

        var routeA = graph.FindRoute(start, end);

        graph.SetConnection(4673, 1371);
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
        var graph = new RoutingGraph <int>();
        const int start = 7216;
        const int end = 5666;
        graph.SetConnections(
            (start, 4673),
            (4673, 3416),
            (4673, 4756),
            (4673, 9713),
            (4756, 1371),
            (9713, 1371),
            (1371, 3464),
            (3464, 2656),
            (3464, end));
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
        var graph = new RoutingGraph <int>();
        graph.SetConnections(
            (0, 1),
            (1, 2),
            (2, 3),
            (3, 0));
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
    public void FindTrivialCycle()
    {
        var graph = new RoutingGraph <int>(true);
        graph.SetConnections(
            (0, 1),
            (1, 0));
        graph.Solve();

        var cycles = graph.GetCycles();

        Assert.AreEqual(2, cycles.Length);
        var cycleA = cycles[0];
        var cycleB = cycles[1];

        Assert.AreEqual(2, cycleA.Length);
        Assert.AreEqual(2, cycleB.Length);

        Assert.AreEqual(0, cycleA[0]);
        Assert.AreEqual(1, cycleA[1]);

        Assert.AreEqual(1, cycleB[0]);
        Assert.AreEqual(0, cycleB[1]);
    }

    [Test]
    public void FindShortCycle()
    {
        var graph = new RoutingGraph <int>(true);
        graph.SetConnections(
            (0, 1),
            (1, 2),
            (2, 0));
        graph.Solve();

        var cycles = graph.GetCycles();

        Assert.AreEqual(3, cycles.Length);
        foreach (var cycle in cycles)
        {
            Assert.AreEqual(3, cycle.Length);
        }
    }

    [Test]
    public void FindShortCycle2()
    {
        var graph = new RoutingGraph <int>(true);
        graph.SetConnections(
            (0, 1),
            (1, 2),
            (2, 0),
            (1, 3));
        graph.Solve();

        var cycles = graph.GetCycles();

        Assert.AreEqual(3, cycles.Length);
        foreach (var cycle in cycles)
        {
            Assert.AreEqual(3, cycle.Length);
        }
    }

    [Test]
    public void DetourExpensiveRoute()
    {
        var graph = new RoutingGraph <int>();
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
        var graph1 = new RoutingGraph <string>();
        graph1.SetConnections(
            ("a", "b"),
            ("b", "c"),
            ("c", "d"),
            ("a", "d"));
        graph1.Solve();

        var graph2 = graph1.Clone();

        CheckGraphs("Clone", graph1, graph2);
    }

    [Test]
    public void Serialization()
    {
        var factory = new JsonFactory<RoutingGraph<int>>();
        var graph = new RoutingGraph <int>();
        const int start = 7216;
        const int end = 5666;
        graph.SetConnections(
            (start, 4673),
            (4673, 3416),
            (4673, 4756),
            (4673, 9713),
            (4756, 1371),
            (9713, 1371),
            (1371, 3464),
            (3464, 2656),
            (3464, end));
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
}
