using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    public interface IPath<out ValueType>
        where ValueType :
            IComparable<ValueType>,
            IEquatable<ValueType>
    {
        float Cost { get; }

        int Count { get; }

        ValueType Start { get; }

        ValueType End { get; }

        IReadOnlyList<ValueType> Nodes { get; }
    }

    [Serializable]
    internal class Path<ValueType> :
        IPath<ValueType>,
        ISerializable,
        IComparable<Path<ValueType>>
        where ValueType :
            IComparable<ValueType>,
            IEquatable<ValueType>
    {
        public static Path<ValueType> operator +(Path<ValueType> start, Path<ValueType> end)
        {
            return new Path<ValueType>(start.nodes
               .Concat(end.nodes.Skip(1)),
               start.Cost + end.Cost);
        }

        public static Path<ValueType> operator ~(Path<ValueType> path)
        {
            return new Path<ValueType>(path.nodes.Reverse(), path.Cost);
        }

        private readonly ValueType[] nodes;

        internal Path(IEnumerable<ValueType> edges, float cost)
        {
            nodes = edges.ToArray();
            Cost = cost;
        }

        internal Path(ValueType a, ValueType b, float cost)
            : this(new[] { a, b }, cost)
        { }

        protected Path(SerializationInfo info, StreamingContext context)
        {
            Cost = info.GetSingle(nameof(Cost));
            nodes = info.GetValue<ValueType[]>(nameof(nodes));
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

        public IReadOnlyList<ValueType> Nodes
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

        public ValueType Start
        {
            get
            {
                return nodes.Length > 0
                  ? nodes[0]
                  : default;
            }
        }

        public ValueType End
        {
            get
            {
                return nodes.Length > 0
                  ? nodes[nodes.Length - 1]
                  : default;
            }
        }

        public int CompareTo(Path<ValueType> other)
        {
            if (other is null
                || Cost < other.Cost
                || Cost == other.Cost
                    && nodes.Length < other.nodes.Length)
            {
                return -1;
            }
            else if (Cost == other.Cost
                && nodes.Length == other.nodes.Length)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }

        public static bool operator <(Path<ValueType> a, Path<ValueType> b)
        {
            return a is object
                && a.CompareTo(b) < 0;
        }

        public static bool operator >(Path<ValueType> a, Path<ValueType> b)
        {
            return a is object
                && a.CompareTo(b) > 0;
        }

        public static bool operator <=(Path<ValueType> a, Path<ValueType> b)
        {
            return a is object
                && a.CompareTo(b) <= 0;
        }

        public static bool operator >=(Path<ValueType> a, Path<ValueType> b)
        {
            return a is object
                && a.CompareTo(b) >= 0;
        }

        public static bool operator ==(Path<ValueType> a, Path<ValueType> b)
        {
            return a is object
                && a.CompareTo(b) == 0;
        }

        public static bool operator !=(Path<ValueType> a, Path<ValueType> b)
        {
            return a is object
                && a.CompareTo(b) != 0;
        }

        public override bool Equals(object obj)
        {
            return obj is Path<ValueType> other
                && Equals(other);
        }

        public override int GetHashCode()
        {
            int code = 0;
            foreach (var x in nodes)
            {
                code ^= x.GetHashCode();
            }
            return code;
        }

        public bool Contains(ValueType x)
        {
            return nodes.Contains(x);
        }

        public override string ToString()
        {
            var pathString = string.Join(" -> ", nodes);
            return $"[{Cost}] {pathString}";
        }
    }
}