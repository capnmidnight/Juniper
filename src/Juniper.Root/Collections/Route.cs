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
        public static Route<ValueT> operator +(Route<ValueT> route, Route<ValueT> extension)
        {
            var newNodes = route.nodes.Concat(extension.nodes.Skip(1));
            return new Route<ValueT>(newNodes, route.Cost + extension.Cost);
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

        public static bool operator >(Route<ValueT> left, Route<ValueT> right)
        {
            return left is object && left.CompareTo(right) == 1
                || right is object && right.CompareTo(left) == -1;
        }

        public static bool operator <(Route<ValueT> left, Route<ValueT> right)
        {
            return left is object && left.CompareTo(right) == -1
                || right is object && right.CompareTo(left) == 1;
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

        public virtual int CompareTo(IRoute<ValueT> other)
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
            else if (Start.Equals(other.End))
            {
                return End.CompareTo(other.Start);
            }
            else if (End.Equals(other.Start))
            {
                return Start.CompareTo(other.End);
            }
            else if (Start.Equals(other.Start))
            {
                return End.CompareTo(other.End);
            }
            else
            {
                return Start.CompareTo(other.Start);
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

        public bool Contains(Route<ValueT> other)
        {
            return Contain(other)
                || Contain(~other);
        }

        private bool Contain(Route<ValueT> other)
        {
            var a = this;
            var b = other;
            var delta = a.Count - b.Count;
            if (delta < 0)
            {
                a = other;
                b = this;
                delta = -delta;
            }

            for (int i = 0; i <= delta; ++i)
            {
                var matches = true;
                for (int j = 0; j < b.Count && matches; ++j)
                {
                    matches &= a.nodes[i + j].Equals(b.Nodes[j]);
                }

                if (matches)
                {
                    return true;
                }
            }

            return false;
        }

        public override string ToString()
        {
            var pathString = string.Join(" -> ", nodes);
            return $"[{Cost}] {pathString}";
        }
    }
}