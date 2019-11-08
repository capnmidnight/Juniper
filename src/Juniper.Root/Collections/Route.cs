using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class Route<ValueT> :
        IRoute<ValueT>,
        ISerializable,
        IEquatable<Route<ValueT>>,
        IComparable<Route<ValueT>>
        where ValueT : IComparable<ValueT>
    {
        public static Route<ValueT> operator +(Route<ValueT> left, Route<ValueT> right)
        {
            if (left is null)
            {
                throw new ArgumentNullException(nameof(left));
            }

            if (right is null)
            {
                throw new ArgumentNullException(nameof(right));
            }

            if (!left.CanConnectTo(right))
            {
                if (left.IntersectsWith(right))
                {
                    throw new InvalidOperationException($"The provided routes overlap:\n\t{left}\n\t{right}");
                }
                else if (left.IsParallelTo(right))
                {
                    throw new InvalidOperationException($"The provided routes start and end at the same location:\n\t{left}\n\t{right}");
                }
                else
                {
                    throw new InvalidOperationException($"The ends of the provided routes do not match:\n\t{left}\n\t{right}");
                }
            }

            var newNodes = new ValueT[left.nodes.Length + right.nodes.Length - 1];
            var reverseNodes = left.Start.Equals(right.Start) || left.Start.Equals(right.End);
            var reverseExtra = left.Start.Equals(right.Start) || left.End.Equals(right.End);
            reverseExtra = reverseExtra != reverseNodes;

            Array.Copy(left.nodes, newNodes, left.nodes.Length);
            if (reverseNodes)
            {
                Array.Reverse(newNodes, 0, left.nodes.Length);
            }

            Array.Copy(right.nodes, reverseExtra ? 0 : 1, newNodes, left.nodes.Length, right.nodes.Length - 1);
            if (reverseExtra)
            {
                Array.Reverse(newNodes, left.nodes.Length, right.nodes.Length - 1);
            }

            return new Route<ValueT>(newNodes, left.Cost + right.Cost);
        }

        public static Route<ValueT> operator ~(Route<ValueT> path)
        {
            return new Route<ValueT>(path.nodes.Reverse(), path.Cost);
        }

        public static bool operator ==(Route<ValueT> left, Route<ValueT> right)
        {
            return left is null && right is null
                || left is object && left.CompareTo(right) == 0
                || right is object && right.CompareTo(left) == 0;
        }

        public static bool operator !=(Route<ValueT> left, Route<ValueT> right)
        {
            return !(left == right);
        }

        public static bool operator <(Route<ValueT> left, Route<ValueT> right)
        {
            return left is object && left.CompareTo(right) == -1
                || right is object && right.CompareTo(left) == 1;
        }

        public static bool operator >(Route<ValueT> left, Route<ValueT> right)
        {
            return left is object && left.CompareTo(right) == 1
                || right is object && right.CompareTo(left) == -1;
        }

        private readonly ValueT[] nodes;

        private Route(IEnumerable<ValueT> edges, float cost)
        {
            nodes = edges.ToArray();

            if (nodes.Length < 2)
            {
                throw new InvalidOperationException("Route must have more than 1 node.");
            }

            for (var i = 0; i < nodes.Length - 1; ++i)
            {
                for (var j = i + 1; j < nodes.Length; ++j)
                {
                    if (nodes[i].Equals(nodes[j]))
                    {
                        throw new InvalidOperationException($"Edges must be a collection of distinct values. Found a duplicate at indices {i} and {j}");
                    }
                }
            }
            Cost = cost;
        }

        public Route(float cost, params ValueT[] nodes)
            : this(nodes, cost)
        { }

        protected Route(SerializationInfo info, StreamingContext context)
        {
            Cost = info.GetSingle(nameof(Cost));
            nodes = info.GetValue<ValueT[]>(nameof(nodes));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(nameof(Cost), Cost);
            info.AddValue(nameof(nodes), nodes);
        }

        public float Cost
        {
            get;
        }

        public IReadOnlyList<ValueT> Nodes
        {
            get
            {
                return nodes;
            }
        }

        public int Count
        {
            get
            {
                return nodes.Length;
            }
        }

        public bool IsConnection
        {
            get
            {
                return Count == 2;
            }
        }

        public bool IsPath
        {
            get
            {
                return Count > 2;
            }
        }

        public ValueT Start
        {
            get
            {
                return nodes.FirstOrDefault();
            }
        }

        public ValueT End
        {
            get
            {
                return nodes.LastOrDefault();
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Route<ValueT> other
                && Equals(other);
        }

        public bool Equals(IRoute<ValueT> other)
        {
            return CompareTo(other) == 0;
        }

        public bool Equals(Route<ValueT> other)
        {
            return Equals((IRoute<ValueT>)other);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Route<ValueT>);
        }

        public int CompareTo(IRoute<ValueT> other)
        {
            if (other is null)
            {
                return -1;
            }
            else if (Start.Equals(other.Start) && End.Equals(other.End)
                || Start.Equals(other.End) && End.Equals(other.Start))
            {
                if (Cost == other.Cost)
                {
                    return Count.CompareTo(other.Count);
                }
                else
                {
                    return Cost.CompareTo(other.Cost);
                }
            }
            else
            {
                return 1;
            }
        }

        public int CompareTo(Route<ValueT> other)
        {
            return CompareTo((IRoute<ValueT>)other);
        }

        public override int GetHashCode()
        {
            var hash = Cost.GetHashCode();
            foreach (var node in nodes)
            {
                hash ^= node.GetHashCode();
            }
            return hash;
        }

        public bool Contains(ValueT x)
        {
            return nodes.Contains(x);
        }

        public bool Contains(Route<ValueT> other)
        {
            if (other is null
                || nodes.Length < other.nodes.Length)
            {
                return false;
            }

            var offset = Array.IndexOf(nodes, other.nodes[0]);
            if (offset == -1)
            {
                return false;
            }

            return Contain(other, offset, 1)
                || Contain(other, offset, -1);
        }

        private bool Contain(Route<ValueT> other, int offset, int direction)
        {
            var end = offset + direction * (other.nodes.Length - 1);
            if (!(0 <= end && end < nodes.Length))
            {
                return false;
            }

            for (int a = offset, b = 0; b < other.nodes.Length;
                a += direction,
                ++b)
            {
                if (!nodes[a].Equals(other.nodes[b]))
                {
                    return false;
                }
            }
            return true;
        }

        public bool CanConnectTo(Route<ValueT> other)
        {
            return other is object
                && !IsParallelTo(other)
                && !IntersectsWith(other);
        }

        public bool IsParallelTo(Route<ValueT> right)
        {
            return Start.Equals(right.Start) && End.Equals(right.End)
                || Start.Equals(right.End) && End.Equals(right.Start);
        }

        public bool IntersectsWith(Route<ValueT> other)
        {
            if (other is null)
            {
                return false;
            }
            else
            {
                for (var i = 0; i < nodes.Length; ++i)
                {
                    var a = nodes[i];
                    var isInternalA = i != 0 && i != nodes.Length - 1;
                    for (var j = 0; j < other.nodes.Length; ++j)
                    {
                        var b = other.nodes[j];
                        var isInternalB = j != 0 && j != other.nodes.Length - 1;
                        if (a.Equals(b)
                            && (isInternalA
                                || isInternalB))
                        {
                            return true;
                        }
                    }
                }

                return false;
            }
        }

        public override string ToString()
        {
            var pathString = string.Join(" -> ", nodes);
            return $"[{Cost}] {pathString}";
        }
    }
}