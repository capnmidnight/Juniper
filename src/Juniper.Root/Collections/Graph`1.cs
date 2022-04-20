using Juniper.IO;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class Graph<NodeT> : ISaveable<Graph<NodeT>>
        where NodeT : IComparable<NodeT>
    {
        private readonly List<Route<NodeT>> connections;
        private readonly SparseMatrix<NodeT, Route<NodeT>> network;

        private readonly Dictionary<string, NodeT> namedNodes;
        private readonly Dictionary<NodeT, string> nodeNames;

        private readonly bool directed;

        private bool dirty;

        public Graph(bool directed = false)
        {
            this.directed = directed;
            dirty = false;
            connections = new();
            namedNodes = new();
            nodeNames = new();
            network = new();
        }

        public Graph<NodeT> Clone()
        {
            var graph = new Graph<NodeT>(directed);
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

        protected Graph(SerializationInfo info, StreamingContext context)
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

        private void AddConnection(NodeT start, NodeT end, float cost)
        {
            _ = connections.RemoveAll(connect =>
                connect.Contains(start)
                && connect.Contains(end)
                && (!directed
                    || connect.Ordered(start, end)));

            connections.Add(new Route<NodeT>(cost, start, end));
        }

        public void SetConnection(NodeT start, NodeT end, float cost = 1)
        {
            if (!start.Equals(end))
            {
                AddConnection(start, end, cost);

                ResetNetwork();
            }
        }

        public void SetConnections(params (NodeT start, NodeT end, float cost)[] connections)
        {
            foreach ((var start, var end, var cost) in connections)
            {
                AddConnection(start, end, cost);
            }

            ResetNetwork();
        }

        public void SetConnections(params (NodeT start, NodeT end)[] connections)
        {
            foreach ((var start, var end) in connections)
            {
                AddConnection(start, end, 1);
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
                foreach (var y in network.Cells(node))
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
