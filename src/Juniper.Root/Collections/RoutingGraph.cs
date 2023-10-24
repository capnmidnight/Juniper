using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper.Collections
{
    public static class RoutingGraph
    {
        public static RoutingGraph<KeyT> ToRoutingGraph<NodeT, KeyT, ValueT>(this IEnumerable<NodeT> items,
            Func<NodeT, KeyT> getKey,
            Func<NodeT, KeyT> getParentKey,
            Func<NodeT, float> getCost = null)
        where KeyT : IComparable<KeyT>
        {
            getCost ??= _ => 1;

            var graph = new RoutingGraph<KeyT>(true);

            graph.SetConnections(items
                .Select(item => new RoutingEdge<KeyT>(getParentKey(item), getKey(item), getCost(item)))
                .ToArray()
            );

            return graph;
        }

        public static RoutingGraph<NodeT> Load<NodeT>(FileInfo file)
            where NodeT : IComparable<NodeT>
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException(file.FullName);
            }

            using var stream = file.OpenRead();
            if (file.Matches(MediaType.Application_Json))
            {
                return LoadJSON<NodeT>(stream);
            }
            else
            {
                throw new InvalidOperationException("Don't know how to read the file type.");
            }
        }

        public static RoutingGraph<NodeT> LoadJSON<NodeT>(Stream stream) where NodeT : IComparable<NodeT>
        {
            return Load(new JsonFactory<RoutingGraph<NodeT>>(), stream);
        }

        private static RoutingGraph<NodeT> Load<NodeT, M>(IDeserializer<RoutingGraph<NodeT>, M> deserializer, Stream stream)
            where M : MediaType
            where NodeT : IComparable<NodeT>
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return deserializer.Deserialize(stream);
        }

        public static RoutingGraph<NodeT> Load<NodeT>(string fileName)
            where NodeT : IComparable<NodeT>
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            return Load<NodeT>(new FileInfo(fileName));
        }
    }

    [Serializable]
    public class RoutingGraph<NodeT> : ISaveable<RoutingGraph<NodeT>>
        where NodeT : IComparable<NodeT>
    {
        private readonly List<Route<NodeT>> connections;
        private readonly SparseMatrix<NodeT, Route<NodeT>> network;

        private readonly Dictionary<string, NodeT> namedNodes;
        private readonly Dictionary<NodeT, string> nodeNames;

        private readonly bool directed;

        private bool dirty;

        public RoutingGraph(bool directed = false)
        {
            this.directed = directed;
            dirty = false;
            connections = new();
            namedNodes = new();
            nodeNames = new();
            network = new();
        }

        public RoutingGraph<NodeT> Clone()
        {
            var graph = new RoutingGraph<NodeT>(directed);
            graph.connections.AddRange(Connections.Distinct());
            foreach (var (x, y, value) in network.Entries)
            {
                graph.network.Add(x, y, value);
            }

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

        protected RoutingGraph(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            directed = info.GetBoolean(nameof(directed));
            network = info.GetValue<SparseMatrix<NodeT, Route<NodeT>>>(nameof(network));
            connections = new List<Route<NodeT>>();

            foreach (var route in network.Values)
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
            info.AddValue(nameof(directed), directed);
            info.AddValue(nameof(dirty), dirty);
            info.AddValue(nameof(namedNodes), namedNodes);
            info.AddValue(nameof(network), network);
        }

        public bool IsDirected => directed;

        public IEnumerable<NodeT> Nodes => network.Keys;

        public IEnumerable<Route<NodeT>> Routes => network.Values;

        public IEnumerable<Route<NodeT>> Connections => connections;

        public IEnumerable<Route<NodeT>> Paths => from route in Routes
                                                  where route.IsPath
                                                  select route;

        public bool HasContent => connections.Count > 0;

        private void ResetNetwork()
        {
            network.Clear();

            foreach (var conn in Connections)
            {
                SetRoute(conn);
            }

            dirty = true;
        }

        private void AddConnection(RoutingEdge<NodeT> edge)
        {
            _ = connections.RemoveAll(connect =>
                connect.Contains(edge.From)
                && connect.Contains(edge.To)
                && (!directed
                    || connect.Ordered(edge.From, edge.To)));

            connections.Add(new Route<NodeT>(edge));
        }

        public void SetConnection(RoutingEdge<NodeT> edge)
        {
            if (!edge.From.Equals(edge.To))
            {
                AddConnection(edge);

                ResetNetwork();
            }
        }

        public void SetConnections(params RoutingEdge<NodeT>[] connections)
        {
            foreach (var edge in connections)
            {
                AddConnection(edge);
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

                network.RemoveColumn(node);
                network.RemoveRow(node);

                dirty = true;
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
            if (network is not null)
            {
                var arr = toRemove.ToArray();
                dirty |= arr.Length > 0;
                foreach (var r in arr)
                {
                    if (Exists(r.Start, r.End))
                    {
                        network[r.Start, r.End] = null;
                    }
                }
            }
        }

        public bool Exists(NodeT node)
        {
            return network.ContainsColumn(node)
                || network.ContainsRow(node);
        }

        public bool Exists(NodeT startPoint, NodeT endPoint)
        {
            return !startPoint.Equals(endPoint)
                && Exists(startPoint)
                && Exists(endPoint)
                && network[startPoint, endPoint] is not null;
        }

        public IEnumerable<Route<NodeT>> FindConnectionsContaining(NodeT node)
        {
            return from conn in Connections
                   where conn.Contains(node)
                   select conn;
        }

        public IEnumerable<Route<NodeT>> FindConnectables(Route<NodeT> route)
        {
            return from conn in Connections
                   where route.CanConnectTo(conn, directed)
                   select conn;
        }

        public IEnumerable<NodeT> FindExits(NodeT node)
        {
            return (from route in FindConnectionsContaining(node)
                    let isReverse = route.End.Equals(node)
                    where !directed || !isReverse
                    select isReverse
                     ? route.Start
                     : route.End)
                    .Distinct();
        }

        public IEnumerable<Route<NodeT>> FindRoutes(NodeT node)
        {
            if (Exists(node))
            {
                foreach (var y in network.ColumnCells(node))
                {
                    yield return network[node, y];
                }
            }
        }

        public Route<NodeT> FindRoute(NodeT startPoint, NodeT endPoint)
        {
            return Exists(startPoint, endPoint)
                ? network[startPoint, endPoint]
                : default;
        }

        private void SetRoute(Route<NodeT> route)
        {
            network[route.Start, route.End] = route;
            if (!directed)
            {
                network[route.End, route.Start] = ~route;
            }
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
            if (!dirty)
            {
                return false;
            }

            var q = new Queue<Route<NodeT>>(connections);
            while (q.Count > 0)
            {
                var route = q.Dequeue();
                foreach (var extension in FindConnectables(route))
                {
                    var next = route.Join(extension, directed);
                    var cur = FindRoute(next.Start, next.End);
                    if (next < cur)
                    {
                        SetRoute(next);
                        q.Add(next);
                    }
                }
            }

            dirty = false;
            return true;
        }

        public NodeT[][] GetCycles()
        {
            var cycles = new List<NodeT[]>();
            foreach (var node in network.Keys)
            {
                foreach (var subNode in FindExits(node))
                {
                    var route = FindRoute(subNode, node);
                    if (route is not null
                        && !route.Start.Equals(node))
                    {
                        var cycle = new List<NodeT>();
                        for (var i = 0; i < route.Count; ++i)
                        {
                            cycle.Add(route.Nodes[(i + 1) % route.Count]);
                        }

                        cycles.Add(cycle.ToArray());
                    }
                }
            }

            return cycles.ToArray();
        }
    }
}
