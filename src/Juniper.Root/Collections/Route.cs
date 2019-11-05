using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class Route<ValueT> :
        IRoute<ValueT>,
        ISerializable
        where ValueT : IComparable<ValueT>
    {
        private static bool CheckOverlap(ValueT[] nodesA, ValueT[] nodesB, int i, int start, int delta)
        {
            for (int j = start + delta;
                0 <= j && j < nodesB.Length && i < nodesA.Length;
                j += delta, ++i)
            {
                var a = nodesA[i];
                var b = nodesB[j];
                if (a.Equals(b))
                {
                    return true;
                }
            }

            return false;
        }

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
                throw new InvalidOperationException("The ends of the provided routes do not match");
            }

            return left.Extend(
                right.nodes, right.Cost,
                left.Start.Equals(right.Start) || left.Start.Equals(right.End),
                left.Start.Equals(right.Start) || left.End.Equals(right.End));
        }

        private Route<ValueT> Extend(ValueT[] extraNodes, float extraCost, bool reverseNodes, bool reverseExtra)
        {
            var newNodes = new ValueT[nodes.Length + extraNodes.Length - 1];

            Array.Copy(nodes, newNodes, nodes.Length);
            if (reverseNodes)
            {
                Array.Reverse(newNodes, 0, nodes.Length);
            }

            reverseExtra = reverseExtra != reverseNodes;
            Array.Copy(extraNodes, reverseExtra ? 0 : 1, newNodes, nodes.Length, extraNodes.Length - 1);
            if (reverseExtra)
            {
                Array.Reverse(newNodes, nodes.Length, extraNodes.Length - 1);
            }

            return new Route<ValueT>(newNodes, Cost + extraCost);
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
            Cost = cost;
        }

        public Route(ValueT a, ValueT b, float cost)
            : this(new[] { a, b }, cost)
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

        public override int GetHashCode()
        {
            return nodes.GetHashCode()
                ^ Cost.GetHashCode();
        }

        public bool Contains(ValueT x)
        {
            return nodes.Contains(x);
        }

        public bool Overlaps(Route<ValueT> other)
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
                    var start = Array.IndexOf(other.nodes, a);
                    if (start > -1)
                    {
                        return CheckOverlap(nodes, other.nodes, i + 1, start, 1)
                            || CheckOverlap(nodes, other.nodes, i + 1, start, -1);
                    }
                }

                return false;
            }
        }

        public bool CanConnectTo(Route<ValueT> other)
        {
            if (other is null)
            {
                return false;
            }
            else
            {
                return (Start.Equals(other.Start)
                        || End.Equals(other.End)
                        || Start.Equals(other.End)
                        || End.Equals(other.Start))
                    && !Overlaps(other);
            }
        }

        public override string ToString()
        {
            var pathString = string.Join(" -> ", nodes);
            return $"[{Cost}] {pathString}";
        }
    }
}