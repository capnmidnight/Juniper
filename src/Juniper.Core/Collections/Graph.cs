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

        private bool dirty;

        private readonly List<NodeT> endPoints;

        private readonly Dictionary<string, NodeT> namedEndPoints;

        private readonly Network network;

        public Graph()
        {
            dirty = false;
            endPoints = new List<NodeT>();
            namedEndPoints = new Dictionary<string, NodeT>();
            network = new Network();
        }

        protected Graph(SerializationInfo info, StreamingContext context)
        {
            dirty = info.GetBoolean(nameof(dirty));
            endPoints = info.GetList<NodeT>(nameof(endPoints));
            namedEndPoints = info.GetValue<Dictionary<string, NodeT>>(nameof(namedEndPoints));

            network = new Network();
            var routes = info.GetValue<Route<NodeT>[]>(nameof(network));
            foreach (var route in routes)
            {
                if (!network.ContainsKey(route.Start))
                {
                    network[route.Start] = new Schedule();
                }

                network[route.Start][route.End] = route;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(dirty), dirty);
            info.AddList(nameof(endPoints), endPoints);
            info.AddValue(nameof(namedEndPoints), namedEndPoints);
            var routes = (from x in network
                          from y in x.Value
                          select y.Value)
                .ToArray();
            info.AddValue(nameof(network), routes);
        }

        private void FillMatrix(NodeT startPoint)
        {
            if (!network.ContainsKey(startPoint))
            {
                network[startPoint] = new Schedule();
            }
        }

        private void Add(Route<NodeT> nextRoute)
        {
            AddSingle(nextRoute);
            AddSingle(~nextRoute);
        }

        private void AddSingle(Route<NodeT> nextRoute)
        {
            network[nextRoute.Start][nextRoute.End] = nextRoute;
        }

        public bool Exists(NodeT startPoint, NodeT endPoint)
        {
            return network.ContainsKey(startPoint)
                && network[startPoint].ContainsKey(endPoint);
        }

        public IRoute<NodeT> GetRoute(NodeT startPoint, NodeT endPoint)
        {
            return Exists(startPoint, endPoint)
                ? network[startPoint][endPoint]
                : null;
        }

        public IRoute<NodeT> GetNamedRoute(NodeT startPoint, string endPointName)
        {
            var endPoint = GetEndPoint(endPointName);
            if(endPoint == default)
            {
                return default;
            }
            else
            {
                return GetRoute(startPoint, endPoint);
            }
        }

        public void Connect(NodeT startPoint, NodeT endPoint, float cost)
        {
            dirty = true;

            FillMatrix(startPoint);
            FillMatrix(endPoint);

            var nextRoute = new Route<NodeT>(startPoint, endPoint, cost);
            if (Exists(startPoint, endPoint))
            {
                var toRemove = new List<Route<NodeT>>();
                foreach (var schedule in network.Values)
                {
                    foreach (var path in schedule.Values)
                    {
                        if (path.Contains(startPoint)
                            && path.Contains(endPoint))
                        {
                            toRemove.Add(path);
                        }
                    }
                }

                foreach (var path in toRemove)
                {
                    network[path.Start].Remove(path.End);
                }
            }

            Add(nextRoute);
        }

        public void AddEndPoint(NodeT endPoint)
        {
            if (!endPoints.Contains(endPoint))
            {
                dirty = true;
                endPoints.Add(endPoint);
            }
        }

        public void AddEndPoint(NodeT endPoint, string name)
        {
            AddEndPoint(endPoint);
            Name(endPoint, name);
        }

        public void Name(NodeT endPoint, string name)
        {
            namedEndPoints[name] = endPoint;
        }

        public IReadOnlyDictionary<string, NodeT> NamedEndPoints
        {
            get
            {
                return namedEndPoints;
            }
        }

        public NodeT GetEndPoint(string name)
        {
            if (namedEndPoints.ContainsKey(name))
            {
                return namedEndPoints[name];
            }
            else
            {
                return default;
            }
        }

        public void Solve()
        {
            if (dirty)
            {
                var q = new Queue<Route<NodeT>>(
                    from endPoint in endPoints
                    let schedule = network[endPoint]
                    from path in schedule.Values
                    select path);

                while (q.Count > 0)
                {
                    var curPath = q.Dequeue();
                    var middle = curPath.End;
                    foreach (var extension in network[middle].Values)
                    {
                        var start = curPath.Start;
                        var end = extension.End;
                        var isBest = !Exists(start, end)
                            || curPath.Cost + extension.Cost < network[start][end].Cost;
                        if (isBest)
                        {
                            var nextPath = curPath + extension;
                            Add(nextPath);
                            q.Enqueue(nextPath);
                        }
                    }
                }

                dirty = false;
            }
        }
    }
}
