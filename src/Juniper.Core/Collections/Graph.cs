using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class Graph<ValueType> :
        ISerializable
        where ValueType :
            IComparable<ValueType>,
            IEquatable<ValueType>
    {
        private bool dirty;

        private readonly Dictionary<ValueType, Dictionary<ValueType, Path<ValueType>>> network;
        private readonly List<ValueType> endPoints;

        public Graph()
        {
            dirty = false;
            endPoints = new List<ValueType>();
            network = new Dictionary<ValueType, Dictionary<ValueType, Path<ValueType>>>();
        }

        protected Graph(SerializationInfo info, StreamingContext context)
        {
            dirty = info.GetBoolean(nameof(dirty));
            endPoints = info.GetList<ValueType>(nameof(endPoints));
            network = new Dictionary<ValueType, Dictionary<ValueType, Path<ValueType>>>();
            var paths = info.GetValue<Path<ValueType>[]>(nameof(network));
            foreach (var path in paths)
            {
                if (!network.ContainsKey(path.Start))
                {
                    network[path.Start] = new Dictionary<ValueType, Path<ValueType>>();
                }

                network[path.Start][path.End] = path;
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(dirty), dirty);
            info.AddList(nameof(endPoints), endPoints);
            var paths = (from x in network
                         from y in x.Value
                         select y.Value)
                .ToArray();
            info.AddValue(nameof(network), paths);
        }

        public void AddEndPoint(ValueType endPoint)
        {
            if (!endPoints.Contains(endPoint))
            {
                dirty = true;
                endPoints.Add(endPoint);
            }
        }

        public IPath<ValueType> this[ValueType startPoint, ValueType endPoint]
        {
            get
            {
                return PathExists(startPoint, endPoint)
                    ? network[startPoint][endPoint]
                    : null;
            }
        }

        public bool PathExists(ValueType startPoint, ValueType endPoint)
        {
            return network.ContainsKey(startPoint)
                && network[startPoint].ContainsKey(endPoint);
        }

        private bool IsBest(Path<ValueType> nextPath)
        {
            return !PathExists(nextPath.Start, nextPath.End)
                || nextPath < network[nextPath.Start][nextPath.End];
        }

        public void Connect(ValueType startPoint, ValueType endPoint, float cost)
        {
            var nextPath = new Path<ValueType>(startPoint, endPoint, cost);
            if (IsBest(nextPath))
            {
                dirty = true;
                AddPath(nextPath);
            }
        }

        private void AddPath(Path<ValueType> nextPath)
        {
            AddSinglePath(nextPath);
            AddSinglePath(~nextPath);
        }

        private void AddSinglePath(Path<ValueType> nextPath)
        {
            if (!network.ContainsKey(nextPath.Start))
            {
                network[nextPath.Start] = new Dictionary<ValueType, Path<ValueType>>();
            }

            network[nextPath.Start][nextPath.End] = nextPath;
        }

        public TimeSpan TimeSpent { get; private set; }

        public void Solve()
        {
            if (dirty)
            {
                var start = DateTime.Now;
                var startPoints = network.Keys.ToArray();
                foreach (var startPoint in network)
                {
                    if (startPoint.Value.Count > 1
                        && !endPoints.Contains(startPoint.Key))
                    {
                        endPoints.Add(startPoint.Key);
                    }
                }

                foreach (var startA in startPoints)
                {
                    var pathsA = network[startA];
                    foreach (var endA in startPoints)
                    {
                        if (!startA.Equals(endA)
                            && pathsA.ContainsKey(endA))
                        {
                            var pathA = pathsA[endA];
                            var pathsB = network[endA];
                            foreach (var endB in startPoints)
                            {
                                if (pathsB.ContainsKey(endB)
                                    && !pathA.Contains(endB))
                                {
                                    var pathB = pathsB[endB];
                                    if (endPoints.Contains(pathB.End))
                                    {
                                        var pathC = pathA + pathB;
                                        if (IsBest(pathC))
                                        {
                                            AddPath(pathC);
                                        }
                                    }
                                }
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
