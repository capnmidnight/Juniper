using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Juniper.Collections
{
    public interface IRoute<out ValueType>
    {
        float Cost { get; }

        int Count { get; }

        ValueType Start { get; }

        ValueType End { get; }

        IReadOnlyList<ValueType> Nodes { get; }
    }

    [Serializable]
    internal class Route<ValueType> :
        IRoute<ValueType>,
        ISerializable
    {
        public static Route<ValueType> operator +(Route<ValueType> route, Route<ValueType> extension)
        {
            var newNodes = route.nodes.Concat(extension.nodes.Skip(1));
            return new Route<ValueType>(newNodes, route.Cost + extension.Cost);
        }

        public static Route<ValueType> operator ~(Route<ValueType> path)
        {
            return new Route<ValueType>(path.nodes.Reverse(), path.Cost);
        }

        private readonly ValueType[] nodes;

        internal Route(IEnumerable<ValueType> edges, float cost)
        {
            nodes = edges.ToArray();
            Cost = cost;
        }

        internal Route(ValueType a, ValueType b, float cost)
            : this(new[] { a, b }, cost)
        { }

        protected Route(SerializationInfo info, StreamingContext context)
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

        public override bool Equals(object obj)
        {
            return obj is Route<ValueType> other
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