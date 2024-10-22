namespace Juniper.Collections.Tests;

public class GraphTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        var graph = new Graph<string, int>();
        graph.Add(new Edge<string, int>("A", "B", 0));
        graph.Add(new Edge<string, int>("A", "C", 0));
        graph.Add(new Edge<string, int>("D", "E", 0));
        var edges = graph.Flood("A");
        var visited = edges
                .Select(v => v.From)
                .Union(edges.Select(v => v.To))
                .Distinct()
                .ToHashSet();
        Assert.Multiple(() =>
        {
            Assert.That(visited, Does.Contain("A"));
            Assert.That(visited, Does.Contain("B"));
            Assert.That(visited, Does.Contain("C"));

            Assert.That(visited, Does.Not.Contain("D"));
            Assert.That(visited, Does.Not.Contain("E"));
        });
    }

    [Test]
    public void Test2()
    {
        var graph = new Graph<string, int>();
        graph.Add(new Edge<string, int>("A", "B", 0));
        graph.Add(new Edge<string, int>("A", "C", 0));
        graph.Add(new Edge<string, int>("D", "E", 0));
        var edges = graph.Flood("D");
        var visited = edges
                .Select(v => v.From)
                .Union(edges.Select(v => v.To))
                .Distinct()
                .ToHashSet();
        Assert.Multiple(() =>
        {
            Assert.That(visited, Does.Not.Contain("A"));
            Assert.That(visited, Does.Not.Contain("B"));
            Assert.That(visited, Does.Not.Contain("C"));

            Assert.That(visited, Does.Contain("D"));
            Assert.That(visited, Does.Contain("E"));
        });
    }
}