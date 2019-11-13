using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper.Collections
{
    [Serializable]
    public class Graph<NodeT> : ISaveable<Graph<NodeT>>
        where NodeT : IComparable<NodeT>
    {
        public static Graph<NodeT> Load(FileInfo file)
        {
            if (MediaType.Application.Json.Matches(file))
            {
                var json = new JsonFactory<Graph<NodeT>>();
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return json.Deserialize(stream);
                }
            }
            else if (MediaType.Application.Octet_Stream.Matches(file))
            {
                var bin = new BinaryFactory<Graph<NodeT>>();
                using (var stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    return bin.Deserialize(stream);
                }
            }
            else
            {
                return null;
            }
        }

        public static Graph<NodeT> Load(string path)
        {
            return Load(new FileInfo(path));
        }

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
        }

        public Graph<NodeT> Clone()
        {
            var graph = new Graph<NodeT>();

            foreach (var node in Nodes)
            {
                graph.AddNode(node);
            }

            graph.connections.AddRange(Connections);
            graph.network = new Route<NodeT>[nodes.Count, nodes.Count];

            foreach (var route in Routes)
            {
                graph.network[graph.nodes[route.Start], graph.nodes[route.End]] = route;
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
                    if (route.IsConnection)
                    {
                        connections.Add(route);
                    }

                    network[nodes[route.Start], nodes[route.End]] = route;
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

            if (namedNodes == null)
            {
                namedNodes = new Dictionary<string, NodeT>();
                nodeNames = new Dictionary<NodeT, string>();
            }
            else
            {
                nodeNames = namedNodes.Invert();
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize only the minimal information that we need to restore
            // the graph.
            info.AddValue(nameof(dirty), dirty);
            info.AddValue(nameof(namedNodes), namedNodes);
            info.AddValue(nameof(network), Routes.ToArray());
        }

        public IEnumerable<NodeT> Nodes
        {
            get
            {
                return nodes.Keys;
            }
        }

        public IEnumerable<Route<NodeT>> Routes
        {
            get
            {
                if (nodes != null)
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

        public IEnumerable<Route<NodeT>> Connections
        {
            get
            {
                return connections;
            }
        }

        public IEnumerable<Route<NodeT>> Paths
        {
            get
            {
                return from route in Routes
                       where route.IsPath
                       select route;
            }
        }

        private void AddNode(NodeT node)
        {
            if (!nodes.ContainsKey(node))
            {
                dirty = true;
                nodes.Add(node, nodes.Count);
            }
        }

        public void Connect(NodeT start, NodeT end, float cost)
        {
            if (!start.Equals(end))
            {
                AddNode(start);
                AddNode(end);

                connections.RemoveAll(connect => connect.Contains(start) && connect.Contains(end));
                connections.Add(new Route<NodeT>(cost, start, end));

                dirty = true;
            }
        }

        public void Remove(NodeT node)
        {
            if (Exists(node))
            {
                Remove(from route in Routes
                       where route.Contains(node)
                       select route);

                for (int i = connections.Count - 1; i >= 0; --i)
                {
                    var connect = connections[i];
                    if (connect.Contains(node))
                    {
                        connections.RemoveAt(i);
                    }
                }

                var index = nodes[node];
                nodes.Remove(node);
                foreach (var key in nodes.Keys.ToArray())
                {
                    if (nodes[key] > index)
                    {
                        --nodes[key];
                    }
                }

                dirty = true;
            }
        }

        public void Remove(NodeT start, NodeT end)
        {
            Remove(from connect in connections
                   where connect.Contains(start)
                    && connect.Contains(end)
                   from route in Routes
                   where route.Contains(connect)
                   select route);

            for (int i = connections.Count - 1; i >= 0; --i)
            {
                var connect = connections[i];
                if (connect.Contains(start)
                    && connect.Contains(end))
                {
                    connections.RemoveAt(i);
                    dirty = true;
                }
            }
        }

        private void Remove(IEnumerable<Route<NodeT>> toRemove)
        {
            var arr = toRemove.ToArray();
            dirty |= arr.Length > 0;
            foreach (var r in arr)
            {
                network[nodes[r.Start], nodes[r.End]] = null;
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
                && !dirty
                && network[nodes[startPoint], nodes[endPoint]] is object;
        }

        private IEnumerable<Route<NodeT>> GetConnections(NodeT node)
        {
            return from connect in Connections
                   where connect.Contains(node)
                   select connect;
        }

        private IEnumerable<Route<NodeT>> GetConnections(Route<NodeT> route)
        {
            return Connections.Where(route.CanConnectTo);
        }

        public IEnumerable<NodeT> GetExits(NodeT node)
        {
            return from route in GetConnections(node)
                   let isReverse = route.End.Equals(node)
                   select isReverse
                    ? route.Start
                    : route.End;
        }

        public IEnumerable<Route<NodeT>> GetRoutes(NodeT node)
        {
            if (Exists(node))
            {
                var x = nodes[node];
                for (int y = 0; y < network.GetLength(1); ++y)
                {
                    var route = network[x, y];
                    if (route is object)
                    {
                        yield return route;
                    }
                }
            }
        }

        public Route<NodeT> GetRoute(NodeT startPoint, NodeT endPoint)
        {
            return Exists(startPoint, endPoint)
                ? network[nodes[startPoint], nodes[endPoint]]
                : default;
        }

        public IReadOnlyDictionary<string, NodeT> NamedNodes
        {
            get
            {
                return namedNodes;
            }
        }

        public IReadOnlyDictionary<NodeT, string> NodeNames
        {
            get
            {
                return nodeNames;
            }
        }

        public string GetNodeName(NodeT node)
        {
            return nodeNames.Get(node);
        }

        public NodeT GetNamedNode(string name)
        {
            return namedNodes.Get(name);
        }

        public void SetNodeName(NodeT node, string name)
        {
            namedNodes[name] = node;
            nodeNames[node] = name;
        }

        public void RemoveNodeName(string name)
        {
            if (namedNodes.ContainsKey(name))
            {
                var node = namedNodes[name];
                namedNodes.Remove(name);
                nodeNames.Remove(node);
            }
        }

        public void RemoveNamedNode(NodeT node)
        {
            if (nodeNames.ContainsKey(node))
            {
                var name = nodeNames[node];
                nodeNames.Remove(node);
                namedNodes.Remove(name);
            }
        }

        public void Solve()
        {
            if (dirty)
            {
                network = new Route<NodeT>[nodes.Count, nodes.Count];

                foreach (var route in Connections)
                {
                    var x = nodes[route.Start];
                    var y = nodes[route.End];
                    network[x, y] = route;
                    network[y, x] = ~route;
                }

                var q = new Queue<Route<NodeT>>(connections);
                while (q.Count > 0)
                {
                    var route = q.Dequeue();
                    foreach (var extension in GetConnections(route))
                    {
                        var next = route + extension;
                        var x = nodes[next.Start];
                        var y = nodes[next.End];
                        var cur = network[x, y];
                        if (next < cur)
                        {
                            network[x, y] = next;
                            network[y, x] = ~next;
                            q.Add(next);
                        }
                    }
                }

                dirty = false;
            }
        }
    }
}
