using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Juniper.Collections
{
    [Serializable]
    public class Graph<NodeT> :
        ISerializable
        where NodeT :
            IComparable<NodeT>,
            IEquatable<NodeT>
    {
        private class Schedule : Dictionary<NodeT, Route<NodeT>> { }

        private class Network : Dictionary<NodeT, Schedule> { }

        private bool dirty;

        private readonly Network network;
        private readonly List<NodeT> endPoints;

        public Graph()
        {
            dirty = false;
            endPoints = new List<NodeT>();
            network = new Network();
        }

        protected Graph(SerializationInfo info, StreamingContext context)
        {
            dirty = info.GetBoolean(nameof(dirty));
            endPoints = info.GetList<NodeT>(nameof(endPoints));
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
            var routes = (from x in network
                          from y in x.Value
                          select y.Value)
                .ToArray();
            info.AddValue(nameof(network), routes);
        }

        public TimeSpan TimeSpent
        {
            get;
            private set;
        }

        private bool IsBest(Route<NodeT> nextRoute)
        {
            return !Exists(nextRoute.Start, nextRoute.End)
                || nextRoute < network[nextRoute.Start][nextRoute.End];
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

        public IRoute<NodeT> this[NodeT startPoint, NodeT endPoint]
        {
            get
            {
                return Exists(startPoint, endPoint)
                    ? network[startPoint][endPoint]
                    : null;
            }
        }

        public void Connect(NodeT startPoint, NodeT endPoint, float cost)
        {
            FillMatrix(startPoint);
            FillMatrix(endPoint);

            var nextRoute = new Route<NodeT>(startPoint, endPoint, cost);
            if (IsBest(nextRoute))
            {
                dirty = true;
                Add(nextRoute);
            }
        }

        public void AddEndPoint(NodeT endPoint)
        {
            if (!endPoints.Contains(endPoint))
            {
                dirty = true;
                endPoints.Add(endPoint);
            }
        }

        public void Solve()
        {
            if (dirty)
            {
                var start = DateTime.Now;

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
                        var nextPath = curPath + extension;
                        if (IsBest(nextPath))
                        {
                            Add(nextPath);
                            q.Enqueue(nextPath);
                        }
                    }
                }

                TimeSpent = DateTime.Now - start;
                dirty = false;
            }
        }
    }
}
