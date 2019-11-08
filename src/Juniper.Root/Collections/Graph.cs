using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class Graph<NodeT> : ISerializable
        where NodeT : IComparable<NodeT>
    {
        private class Schedule : Dictionary<NodeT, Route<NodeT>> { }

        private class Network : Dictionary<NodeT, Schedule> { }

        private readonly List<NodeT> endPoints;
        private readonly Dictionary<string, NodeT> namedEndPoints;
        private readonly Dictionary<NodeT, string> endPointNames;
        private readonly Network network;

        private bool dirty;

        public Graph()
        {
            endPoints = new List<NodeT>();
            namedEndPoints = new Dictionary<string, NodeT>();
            endPointNames = new Dictionary<NodeT, string>();
            network = new Network();
            dirty = false;
        }

        public Graph<NodeT> Clone()
        {
            var graph = new Graph<NodeT>();
            graph.endPoints.AddRange(endPoints);
            foreach (var pair in namedEndPoints)
            {
                graph.namedEndPoints.Add(pair.Key, pair.Value);
                graph.endPointNames.Add(pair.Value, pair.Key);
            }

            foreach (var route in Connections)
            {
                if (!graph.network.ContainsKey(route.Start))
                {
                    graph.network[route.Start] = new Schedule();
                }

                graph.network[route.Start][route.End] = route;
            }

            graph.dirty = true;

            return graph;
        }

        protected Graph(SerializationInfo info, StreamingContext context)
        {
            endPoints = info.GetList<NodeT>(nameof(endPoints));
            namedEndPoints = info.GetValue<Dictionary<string, NodeT>>(nameof(namedEndPoints));
            endPointNames = namedEndPoints.Invert();
            network = new Network();
            var routes = info.GetValue<Route<NodeT>[]>(nameof(network));
            foreach (var route in routes)
            {
                if (route.IsValid)
                {
                    FillNetworks(route);
                }
            }
            dirty = true;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Serialize only the minimal information that we need to restore
            // the graph.
            info.AddList(nameof(endPoints), endPoints);
            info.AddValue(nameof(namedEndPoints), namedEndPoints);
            var routes = (from route in Connections
                          select route)
                        .Distinct()
                        .ToArray();
            info.AddValue(nameof(network), routes);
        }

        public IReadOnlyList<NodeT> EndPoints
        {
            get
            {
                return endPoints;
            }
        }

        public void Connect(NodeT startPoint, NodeT endPoint, float cost)
        {
            if (!startPoint.Equals(endPoint))
            {
                var newRoute = new Route<NodeT>(cost, startPoint, endPoint);
                var curRoute = GetRoute(startPoint, endPoint);
                if (newRoute != curRoute)
                {
                    dirty = true;

                    if (curRoute != null)
                    {
                        RemoveRoutes(from route in Routes
                                     where route.Contains(curRoute)
                                     select route);
                    }

                    FillNetworks(newRoute);
                }
            }
        }

        private void FillNetworks(Route<NodeT> route)
        {
            FillNetwork(route);
            FillNetwork(~route);
        }

        private void FillNetwork(Route<NodeT> route)
        {
            if (!network.ContainsKey(route.Start))
            {
                network[route.Start] = new Schedule();
            }

            network[route.Start][route.End] = route;
        }

        public IEnumerable<Route<NodeT>> Routes
        {
            get
            {
                return from schedule in network.Values
                       from r in schedule.Values
                       select r;
            }
        }

        public IEnumerable<Route<NodeT>> Connections
        {
            get
            {
                return from route in Routes
                       where route.IsConnection
                       select route;
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

        public void Disconnect(NodeT start, NodeT end)
        {
            RemoveRoutes(from connect in Connections
                         where connect.Contains(start)
                            && connect.Contains(end)
                         from route in Routes
                         where route == connect
                            || ((route.Contains(connect.Start)
                                    || route.Contains(connect.End))
                                && route.IntersectsWith(connect))
                         select route);
        }

        public void Remove(NodeT node)
        {
            RemoveRoutes(from route in Routes
                         where route.Contains(node)
                         select route);
        }

        private void RemoveRoutes(IEnumerable<Route<NodeT>> toRemove)
        {
            var arr = toRemove.ToArray();
            dirty |= arr.Length > 0;
            foreach (var r in arr)
            {
                network[r.Start].Remove(r.End);
                if (network[r.Start].Count == 0)
                {
                    network.Remove(r.Start);
                }
            }
        }

        public bool NodeExists(NodeT node)
        {
            return network.ContainsKey(node);
        }

        public bool RouteExists(NodeT startPoint, NodeT endPoint)
        {
            return !startPoint.Equals(endPoint)
                && NodeExists(startPoint)
                && network[startPoint].ContainsKey(endPoint);
        }

        public Route<NodeT> GetRoute(NodeT startPoint, NodeT endPoint)
        {
            return RouteExists(startPoint, endPoint)
                ? network[startPoint][endPoint]
                : default;
        }

        public IEnumerable<Route<NodeT>> GetRoutes(NodeT node)
        {
            if (NodeExists(node))
            {
                foreach (var route in network[node].Values)
                {
                    yield return route;
                }
            }
        }

        public IEnumerable<NodeT> GetConnections(NodeT node)
        {
            foreach (var route in GetRoutes(node))
            {
                if (route.IsConnection)
                {
                    yield return route.End;
                }
            }
        }

        public void AddEndPoint(NodeT endPoint)
        {
            dirty |= endPoints.MaybeAdd(endPoint);
        }

        public void RemoveEndPoint(NodeT endPoint)
        {
            dirty |= endPoints.Remove(endPoint);
        }

        public IReadOnlyDictionary<string, NodeT> NamedEndPoints
        {
            get
            {
                return namedEndPoints;
            }
        }

        public IReadOnlyDictionary<NodeT, string> EndPointNames
        {
            get
            {
                return endPointNames;
            }
        }

        public string GetEndPointName(NodeT node)
        {
            return endPointNames.Get(node);
        }

        public NodeT GetNamedEndPoint(string name)
        {
            return namedEndPoints.Get(name);
        }

        public void SetEndPointName(NodeT endPoint, string name)
        {
            namedEndPoints[name] = endPoint;
            endPointNames[endPoint] = name;
        }

        public void RemoveEndPointName(string name)
        {
            if (namedEndPoints.ContainsKey(name))
            {
                var endPoint = namedEndPoints[name];
                namedEndPoints.Remove(name);
                endPointNames.Remove(endPoint);
            }
        }

        public void Solve()
        {
            if (dirty)
            {
                RemoveRoutes(Paths);

                var q = new Queue<Route<NodeT>>(
                    from endPoint in endPoints
                    from route in GetRoutes(endPoint)
                    select route);

                var toAdd = new List<Route<NodeT>>();

                while (q.Count > 0)
                {
                    var route = q.Dequeue();
                    foreach (var extension in GetRoutes(route.End))
                    {
                        if (route.CanConnectTo(extension))
                        {
                            var nextRoute = route + extension;
                            var curRoute = GetRoute(nextRoute.Start, nextRoute.End);
                            if (nextRoute < curRoute)
                            {
                                toAdd.Add(nextRoute);
                            }
                        }
                    }

                    foreach (var nextRoute in toAdd)
                    {
                        FillNetworks(nextRoute);
                        q.Add(nextRoute);
                    }

                    toAdd.Clear();
                }

                dirty = false;
            }
        }
    }
}
