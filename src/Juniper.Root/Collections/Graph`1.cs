using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper.Collections
{

    [Serializable]
    public class Graph<NodeT> : ISaveable<Graph<NodeT>>
        where NodeT : IComparable<NodeT>
    {
        private readonly Dictionary<NodeT, int> nodes;
        private readonly List<Route<NodeT>> connections;
        private Route<NodeT>[,] network;

        private readonly Dictionary<string, NodeT> namedNodes;
        private readonly Dictionary<NodeT, string> nodeNames;

        private bool dirty;

        public Graph()
        {
            dirty = false;
            nodes = new Dictionary<NodeT, int>();
            connections = new List<Route<NodeT>>();
            namedNodes = new Dictionary<string, NodeT>();
            nodeNames = new Dictionary<NodeT, string>();
            network = new Route<NodeT>[0, 0];
        }

        public Graph<NodeT> Clone()
        {
            var graph = new Graph<NodeT>();

            foreach (var node in Nodes)
            {
                graph.AddNode(node);
            }

            graph.connections.AddRange(Connections.Distinct());
            graph.network = new Route<NodeT>[nodes.Count, nodes.Count];

            foreach (var route in Routes)
            {
                graph.SetRoute(route);
            }

            foreach (var pair in namedNodes)
            {
                graph.namedNodes.Add(pair.Key, pair.Value);
                graph.nodeNames.Add(pair.Value, pair.Key);
            }

            graph.dirty = dirty;

            return graph;
        }

        protected Graph(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            var routes = info.GetValue<Route<NodeT>[]>(nameof(network));

            nodes = new Dictionary<NodeT, int>();

            foreach (var node in (from route in routes
                                  from node in route.Nodes
                                  select node)
                        .Distinct())
            {
                AddNode(node);
            }

            network = new Route<NodeT>[nodes.Count, nodes.Count];
            connections = new List<Route<NodeT>>();

            foreach (var route in routes)
            {
                if (route.IsValid)
                {
                    if (route.IsConnection
                        && !connections.Contains(route))
                    {
                        connections.Add(route);
                    }

                    SetRoute(route);
                }
            }

            dirty = true;

            foreach (var pair in info)
            {
                switch (pair.Name)
                {
                    case nameof(namedNodes):
                    case "namedEndPoints":
                    namedNodes = info.GetValue<Dictionary<string, NodeT>>(pair.Name);
                    break;
                    case nameof(dirty):
                    dirty = info.GetBoolean(nameof(dirty));
                    break;
                }
            }

            if (namedNodes is null)
            {
                namedNodes = new Dictionary<string, NodeT>();
                nodeNames = new Dictionary<NodeT, string>();
            }
            else
            {
                nodeNames = namedNodes.Invert();
            }
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            // Serialize only the minimal information that we need to restore
            // the graph.
            info.AddValue(nameof(dirty), dirty);
            info.AddValue(nameof(namedNodes), namedNodes);
            info.AddValue(nameof(network), Routes.Distinct().ToArray());
        }

        public IEnumerable<NodeT> Nodes => nodes.Keys;

        public IEnumerable<Route<NodeT>> Routes
        {
            get
            {
                if (network is object)
                {
                    for (var x = 0; x < network.GetLength(0); ++x)
                    {
                        for (var y = 0; y < network.GetLength(1); ++y)
                        {
                            if (network[x, y] is object)
                            {
                                yield return network[x, y];
                            }
                        }
                    }
                }
            }
        }

        public IEnumerable<Route<NodeT>> Connections => connections;

        public IEnumerable<Route<NodeT>> Paths
        {
            get
            {
                return from route in Routes
                       where route.IsPath
                       select route;
            }
        }

        public bool HasContent
        {
            get
            {
                return nodes.Count > 0
                    && connections.Count > 0;
            }
        }

        private void AddNode(NodeT node)
        {
            _ = nodes.Default(node, nodes.Count);
        }

        private void ResetNetwork()
        {
            network = new Route<NodeT>[nodes.Count, nodes.Count];

            foreach (var route in Connections)
            {
                SetRoute(route);
            }

            dirty = true;
        }

        private void AddConnection(NodeT start, NodeT end, float cost)
        {
            AddNode(start);
            AddNode(end);

            _ = connections.RemoveAll(connect =>
                connect.Contains(start)
                && connect.Contains(end));

            connections.Add(new Route<NodeT>(cost, start, end));
        }

        public void SetConnection(NodeT start, NodeT end, float cost)
        {
            if (!start.Equals(end))
            {
                AddConnection(start, end, cost);

                ResetNetwork();
            }
        }

        public void SetConnections(params (NodeT start, NodeT end, float cost)[] connections)
        {
            foreach (var (start, end, cost) in connections)
            {
                AddConnection(start, end, cost);
            }

            ResetNetwork();
        }

        public void Remove(NodeT node)
        {
            if (Exists(node))
            {
                Remove(from route in Routes
                       where route.Contains(node)
                       select route);

                for (var i = connections.Count - 1; i >= 0; --i)
                {
                    var connect = connections[i];
                    if (connect.Contains(node))
                    {
                        connections.RemoveAt(i);
                    }
                }

                var index = nodes[node];
                _ = nodes.Remove(node);
                foreach (var key in nodes.Keys.ToArray())
                {
                    if (nodes[key] > index)
                    {
                        --nodes[key];
                    }
                }

                ResetNetwork();
            }
        }

        public void Remove(NodeT start, NodeT end)
        {
            if (Exists(start, end))
            {
                Remove(from connect in connections
                       where connect.Contains(start)
                        && connect.Contains(end)
                       from route in Routes
                       where route.Contains(connect)
                       select route);

                for (var i = connections.Count - 1; i >= 0; --i)
                {
                    var connect = connections[i];
                    if (connect.Contains(start)
                        && connect.Contains(end))
                    {
                        connections.RemoveAt(i);
                    }
                }

                ResetNetwork();
            }
        }

        private void Remove(IEnumerable<Route<NodeT>> toRemove)
        {
            if (network is object)
            {
                var arr = toRemove.ToArray();
                dirty |= arr.Length > 0;
                foreach (var r in arr)
                {
                    if (Exists(r.Start, r.End))
                    {
                        network[nodes[r.Start], nodes[r.End]] = null;
                    }
                }
            }
        }

        public bool Exists(NodeT node)
        {
            return nodes.ContainsKey(node);
        }

        public bool Exists(NodeT startPoint, NodeT endPoint)
        {
            return !startPoint.Equals(endPoint)
                && Exists(startPoint)
                && Exists(endPoint)
                && network[nodes[startPoint], nodes[endPoint]] is object;
        }

        public IEnumerable<Route<NodeT>> FindConnections(NodeT node)
        {
            return from connect in Connections
                   where connect.Contains(node)
                   select connect;
        }

        public IEnumerable<Route<NodeT>> FindConnections(Route<NodeT> route)
        {
            if (route is object)
            {
                return Connections.Where(route.CanConnectTo);
            }
            else
            {
                return Array.Empty<Route<NodeT>>();
            }
        }

        public IEnumerable<NodeT> FindExits(NodeT node)
        {
            return (from route in FindConnections(node)
                    let isReverse = route.End.Equals(node)
                    select isReverse
                     ? route.Start
                     : route.End)
                    .Distinct();
        }

        public IEnumerable<Route<NodeT>> FindRoutes(NodeT node)
        {
            if (Exists(node))
            {
                var x = nodes[node];
                for (var y = 0; y < network.GetLength(1); ++y)
                {
                    var route = network[x, y];
                    if (route is object)
                    {
                        yield return route;
                    }
                }
            }
        }

        public Route<NodeT> FindRoute(NodeT startPoint, NodeT endPoint)
        {
            return Exists(startPoint, endPoint)
                ? network[nodes[startPoint], nodes[endPoint]]
                : default;
        }

        private void SetRoute(Route<NodeT> route)
        {
            var x = nodes[route.Start];
            var y = nodes[route.End];
            network[x, y] = route;
            network[y, x] = ~route;
        }

        public IReadOnlyDictionary<string, NodeT> NamedNodes => namedNodes;

        public IReadOnlyDictionary<NodeT, string> NodeNames => nodeNames;

        public string FindNodeName(NodeT node)
        {
            return nodeNames.Get(node);
        }

        public NodeT FindNamedNode(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return default;
            }
            else
            {
                return namedNodes.Get(name);
            }
        }

        public void SetNodeName(NodeT node, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                namedNodes[name] = node;
                nodeNames[node] = name;
            }
        }

        public void RemoveNodeName(string name)
        {
            if (!string.IsNullOrEmpty(name)
                && namedNodes.ContainsKey(name))
            {
                var node = namedNodes[name];
                _ = namedNodes.Remove(name);
                _ = nodeNames.Remove(node);
            }
        }

        public void RemoveNamedNode(NodeT node)
        {
            if (nodeNames.ContainsKey(node))
            {
                var name = nodeNames[node];
                _ = nodeNames.Remove(node);
                _ = namedNodes.Remove(name);
            }
        }

        public bool Solve()
        {
            var wasDirty = dirty;

            if (dirty)
            {
                var q = new Queue<Route<NodeT>>(connections);
                while (q.Count > 0)
                {
                    var route = q.Dequeue();
                    foreach (var extension in FindConnections(route))
                    {
                        var next = route + extension;
                        var cur = FindRoute(next.Start, next.End);
                        if (next < cur)
                        {
                            SetRoute(next);
                            q.Add(next);
                        }
                    }
                }

                dirty = false;
            }

            return wasDirty;
        }
    }
}
