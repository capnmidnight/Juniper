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
        private readonly Func<ValueType, ValueType, float> costFunc;

        public Graph(Func<ValueType, ValueType, float> costFunc)
        {
            dirty = false;
            endPoints = new List<ValueType>();
            network = new Network();
            this.costFunc = costFunc;
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

        private void ReadOnlyCheck()
        {
            if (costFunc == null)
            {
                throw new InvalidOperationException("This graph is read-only");
            }
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

        public void Connect(ValueType startPoint, ValueType endPoint)
        {
            ReadOnlyCheck();
            FillMatrix(startPoint);
            FillMatrix(endPoint);

            var cost = costFunc(startPoint, endPoint);
            var nextRoute = new Route<ValueType>(startPoint, endPoint, cost);
            if (IsBest(nextRoute))
            {
                dirty = true;
                Add(nextRoute);
            }
        }

        public void AddEndPoint(ValueType endPoint)
        {
            ReadOnlyCheck();
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
            ReadOnlyCheck();
            if (dirty)
            {
                var start = DateTime.Now;

                var q = new Queue<Route<ValueType>>(
                    from endPoint in endPoints
                    from path in network[endPoint].Values
                    select path);

                var shortPaths = new Network();
                foreach (var startPoint in network.Keys)
                {
                    shortPaths[startPoint] = new Schedule();
                    foreach (var endPoint in network[startPoint].Keys)
                    {
                        shortPaths[startPoint][endPoint]
                            = network[startPoint][endPoint];
                    }
                }

                while (q.Count > 0)
                {
                    var curPath = q.Dequeue();
                    if (shortPaths.ContainsKey(curPath.End))
                    {
                        var neighbors = shortPaths[curPath.End].Values;
                        foreach (var neighbor in neighbors)
                        {
                            var additionalCost = costFunc(curPath.End, neighbor.End);
                            var nextPath = curPath.Extend(neighbor.End, additionalCost);
                            if (IsBest(nextPath))
                            {
                                Add(nextPath);
                                q.Enqueue(nextPath);
                            }
                        }
                    }
                }

                TimeSpent = DateTime.Now - start;
                dirty = false;
            }
        }
    }
}
