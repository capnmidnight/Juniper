using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    [Serializable]
    public class Route<ValueT> :
        ISerializable,
        IComparable,
        IComparable<Route<ValueT>>,
        IEquatable<Route<ValueT>>
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
                if (left.Intersects(right))
                {
                    throw new InvalidOperationException($"The provided routes overlap:\n\t{left}\n\t{right}");
                }
                else if (left.Parallels(right))
                {
                    throw new InvalidOperationException($"The provided routes start and end at the same location:\n\t{left}\n\t{right}");
                }
                else
                {
                    throw new InvalidOperationException($"The ends of the provided routes do not match:\n\t{left}\n\t{right}");
                }
            }

            var newNodes = new ValueT[left.Count + right.Count - 1];
            var reverseNodes = left.Start.Equals(right.Start) || left.Start.Equals(right.End);
            var reverseExtra = left.Start.Equals(right.Start) || left.End.Equals(right.End);
            reverseExtra = reverseExtra != reverseNodes;

            Array.Copy(left.nodes, newNodes, left.Count);
            if (reverseNodes)
            {
                Array.Reverse(newNodes, 0, left.Count);
            }

            Array.Copy(
                right.nodes,
                reverseExtra
                    ? 0
                    : 1,
                newNodes,
                left.Count,
                right.Count - 1);

            if (reverseExtra)
            {
                Array.Reverse(newNodes, left.Count, right.Count - 1);
            }

            return new Route<ValueT>(false, newNodes, left.Cost + right.Cost);
        }

        public static Route<ValueT> operator ~(Route<ValueT> path)
        {
            return new Route<ValueT>(true, path.nodes.Reverse(), path.Cost);
        }

        public static bool operator ==(Route<ValueT> left, Route<ValueT> right)
        {
            return (left is null && right is null)
                || (left is object && left.CompareTo(right) == 0)
                || (right is object && right.CompareTo(left) == 0);
        }

        public static bool operator !=(Route<ValueT> left, Route<ValueT> right)
        {
            return !(left == right);
        }

        public static bool operator <(Route<ValueT> left, Route<ValueT> right)
        {
            return (left is object && left.CompareTo(right) == -1)
                || (right is object && right.CompareTo(left) == 1);
        }

        public static bool operator >(Route<ValueT> left, Route<ValueT> right)
        {
            return (left is object && left.CompareTo(right) == 1)
                || (right is object && right.CompareTo(left) == -1);
        }

        public static bool operator <=(Route<ValueT> left, Route<ValueT> right)
        {
            return left < right || left == right;
        }

        public static bool operator >=(Route<ValueT> left, Route<ValueT> right)
        {
            return left > right || left == right;
        }

        private readonly ValueT[] nodes;

        private Route(bool validate, IEnumerable<ValueT> edges, float cost)
        {
            IsValid = true;

            nodes = edges.ToArray();

            if (!validate && nodes.Length < 2)
            {
                IsValid = false;
            }

            for (var i = 0; i < nodes.Length - 1; ++i)
            {
                for (var j = i + 1; j < nodes.Length; ++j)
                {
                    if (nodes[i].Equals(nodes[j]))
                    {
                        if (validate)
                        {
                            throw new InvalidOperationException($"Edges must be a collection of distinct values. Found a duplicate at indices {i.ToString(CultureInfo.CurrentCulture)} and {j.ToString(CultureInfo.CurrentCulture)}");
                        }
                        else
                        {
                            IsValid = false;
                        }
                    }
                }
            }

            Cost = cost;
        }

        public Route(float cost, ValueT firstNode, ValueT secondNode, params ValueT[] nodes)
            : this(true, nodes.Prepend(secondNode).Prepend(firstNode), cost)
        { }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA1801:Review unused parameters", Justification = "Parameter `context` is required by ISerializable interface")]
        protected Route(SerializationInfo info, StreamingContext context)
            : this(false,
                info?.GetValue<ValueT[]>(nameof(nodes)),
                info?.GetSingle(nameof(Cost)) ?? throw new ArgumentNullException(nameof(info)))
        { }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Cost), Cost);
            info.AddValue(nameof(nodes), nodes);
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

        public override string ToString()
        {
            var pathString = string.Join(" -> ", nodes);
            return $"[{Cost.ToString(CultureInfo.CurrentCulture)}] {pathString}";
        }

        public float Cost { get; }

        public IReadOnlyList<ValueT> Nodes
        {
            get { return nodes; }
        }

        public int Count
        {
            get { return nodes.Length; }
        }

        internal bool IsValid { get; }

        public bool IsConnection
        {
            get { return Count == 2; }
        }

        public bool IsPath
        {
            get { return Count > 2; }
        }

        public ValueT Start
        {
            get { return nodes.FirstOrDefault(); }
        }

        public ValueT End
        {
            get { return nodes.LastOrDefault(); }
        }

        public override bool Equals(object obj)
        {
            return obj is Route<ValueT> other
                && Equals(other);
        }

        public bool Equals(Route<ValueT> other)
        {
            return CompareTo(other) == 0;
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as Route<ValueT>);
        }

        public int CompareTo(Route<ValueT> other)
        {
            if (other is null)
            {
                return -1;
            }
            else if (Parallels(other))
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
            else if (End.Equals(other.End))
            {
                return Start.CompareTo(other.Start);
            }
            else
            {
                return End.CompareTo(other.End);
            }
        }

        public bool Contains(ValueT x)
        {
            return nodes.Contains(x);
        }

        public bool Contains(Route<ValueT> other)
        {
            if (other is null
                || nodes.Length < other.Count)
            {
                return false;
            }

            return Contain(other)
                || Contain(~other);
        }

        private bool Contain(Route<ValueT> other)
        {
            var offset = Array.IndexOf(nodes, other.nodes[0]);
            if (offset == -1)
            {
                return false;
            }

            var end = offset + other.Count - 1;
            if (!(0 <= end && end < nodes.Length))
            {
                return false;
            }

            for (var i = 0; i < other.Count; ++i)
            {
                var a = nodes[i + offset];
                var b = other.nodes[i];
                if (!a.Equals(b))
                {
                    return false;
                }
            }

            return true;
        }

        public bool CanConnectTo(Route<ValueT> other)
        {
            return other is object
                && (Start.Equals(other.Start)
                    || Start.Equals(other.End)
                    || End.Equals(other.Start)
                    || End.Equals(other.End))
                && !Parallels(other)
                && !Intersects(other);
        }

        public bool Parallels(Route<ValueT> other)
        {
            return other is object
                && (Parallel(other)
                    || Parallel(~other));
        }

        private bool Parallel(Route<ValueT> other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public bool Intersects(Route<ValueT> other)
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
    }
}