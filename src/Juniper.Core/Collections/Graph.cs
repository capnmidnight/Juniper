using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Juniper.Collections
{
    [Serializable]
    public class Graph<ValueType> :
        ISerializable
        where ValueType :
            IComparable<ValueType>,
            IEquatable<ValueType>
    {
        private class Schedule : Dictionary<ValueType, Route<ValueType>> { }

        private class Network : Dictionary<ValueType, Schedule> { }

        private bool dirty;

        private readonly Network network;
        private readonly List<ValueType> endPoints;

        public Graph()
        {
            dirty = false;
            endPoints = new List<ValueType>();
            network = new Network();
        }

        protected Graph(SerializationInfo info, StreamingContext context)
        {
            dirty = info.GetBoolean(nameof(dirty));
            endPoints = info.GetList<ValueType>(nameof(endPoints));
            network = new Network();
            var routes = info.GetValue<Route<ValueType>[]>(nameof(network));
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

        private bool IsBest(Route<ValueType> nextRoute)
        {
            return !Exists(nextRoute.Start, nextRoute.End)
                || nextRoute < network[nextRoute.Start][nextRoute.End];
        }

        private void FillMatrix(ValueType startPoint)
        {
            if (!network.ContainsKey(startPoint))
            {
                network[startPoint] = new Schedule();
            }
        }

        private void Add(Route<ValueType> nextRoute)
        {
            AddSingle(nextRoute);
            AddSingle(~nextRoute);
        }

        private void AddSingle(Route<ValueType> nextRoute)
        {
            network[nextRoute.Start][nextRoute.End] = nextRoute;
        }

        public bool Exists(ValueType startPoint, ValueType endPoint)
        {
            return network.ContainsKey(startPoint)
                && network[startPoint].ContainsKey(endPoint);
        }

        public IRoute<ValueType> this[ValueType startPoint, ValueType endPoint]
        {
            get
            {
                return Exists(startPoint, endPoint)
                    ? network[startPoint][endPoint]
                    : null;
            }
        }

        public void Connect(ValueType startPoint, ValueType endPoint, float cost)
        {
            FillMatrix(startPoint);
            FillMatrix(endPoint);

            var nextRoute = new Route<ValueType>(startPoint, endPoint, cost);
            if (IsBest(nextRoute))
            {
                dirty = true;
                Add(nextRoute);
            }
        }

        public void AddEndPoint(ValueType endPoint)
        {
            if (!endPoints.Contains(endPoint))
            {
                dirty = true;
                endPoints.Add(endPoint);
            }
        }

        public Task SolveAsync()
        {
            var task = Task.Run(Solve);
            task.ConfigureAwait(false);
            return task;
        }

        public void Solve()
        {
            if (dirty)
            {
                var start = DateTime.Now;

                var q = new Queue<Route<ValueType>>(
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
