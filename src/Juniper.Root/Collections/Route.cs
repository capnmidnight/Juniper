using System.Globalization;
using System.Runtime.Serialization;

namespace Juniper.Collections;


[Serializable]
public class Route<ValueT> :
    ISerializable,
    IComparable,
    IComparable<Route<ValueT>>,
    IEquatable<Route<ValueT>>
    where ValueT : IComparable<ValueT>
{
    public Route<ValueT> Join(Route<ValueT> right, bool directed)
    {
        if (right is null)
        {
            throw new ArgumentNullException(nameof(right));
        }

        if (!CanConnectTo(right, directed))
        {
            if (Intersects(right))
            {
                throw new InvalidOperationException($"The provided routes overlap:\n\t{this}\n\t{right}");
            }
            else if (Parallels(right))
            {
                throw new InvalidOperationException($"The provided routes start and end at the same location:\n\t{this}\n\t{right}");
            }
            else
            {
                throw new InvalidOperationException($"The ends of the provided routes do not match:\n\t{this}\n\t{right}");
            }
        }

        var newNodes = new ValueT[Count + right.Count - 1];
        var reverseNodes = Start.Equals(right.Start) || Start.Equals(right.End);
        var reverseExtra = Start.Equals(right.Start) || End.Equals(right.End);
        reverseExtra = reverseExtra != reverseNodes;

        Array.Copy(nodes, newNodes, Count);
        if (reverseNodes)
        {
            Array.Reverse(newNodes, 0, Count);
        }

        Array.Copy(
            right.nodes,
            reverseExtra
                ? 0
                : 1,
            newNodes,
            Count,
            right.Count - 1);

        if (reverseExtra)
        {
            Array.Reverse(newNodes, Count, right.Count - 1);
        }

        return new Route<ValueT>(false, newNodes, Cost + right.Cost);
    }

    public static Route<ValueT> operator ~(Route<ValueT> path)
    {
        return new Route<ValueT>(true, path.nodes.Reverse(), path.Cost);
    }

    public static bool operator ==(Route<ValueT>? left, Route<ValueT>? right)
    {
        if(left is not null){
            return left.CompareTo(right) == 0;
        }
        else if(right is not null){
            return right.CompareTo(left) == 0;
        }
        else {
            return true;
        }
    }

    public static bool operator !=(Route<ValueT> left, Route<ValueT> right)
    {
        return !(left == right);
    }

    public static bool operator <(Route<ValueT> left, Route<ValueT> right)
    {
        return (left is not null && left.CompareTo(right) == -1)
            || (right is not null && right.CompareTo(left) == 1);
    }

    public static bool operator >(Route<ValueT> left, Route<ValueT> right)
    {
        return (left is not null && left.CompareTo(right) == 1)
            || (right is not null && right.CompareTo(left) == -1);
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

    public Route(Edge<ValueT, float> edge, params ValueT[] nodes)
        : this(true, nodes.Prepend(edge.From).Prepend(edge.To), edge.Value)
    { }

    protected Route(SerializationInfo info, StreamingContext context)
        : this(false,
            info?.GetValue<ValueT[]>(nameof(nodes)) ?? throw new ArgumentNullException(nameof(nodes)),
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

    public override string ToString()
    {
        var pathString = nodes.ToString(" -> ");
        return $"[{Cost.ToString(CultureInfo.CurrentCulture)}] {pathString}";
    }

    public float Cost { get; }

    public IReadOnlyList<ValueT> Nodes => nodes;

    public int Count => nodes.Length;

    internal bool IsValid { get; }

    public bool IsConnection => Count == 2;

    public bool IsPath => Count > 2;

    public ValueT Start => nodes.First();

    public ValueT End => nodes.Last();

    public override bool Equals(object? obj)
    {
        return obj is Route<ValueT> other
            && Equals(other);
    }

    public bool Equals(Route<ValueT>? other)
    {
        return CompareTo(other) == 0;
    }

    public int CompareTo(object? obj)
    {
        return CompareTo(obj as Route<ValueT>);
    }

    public int CompareTo(Route<ValueT>? other)
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

    public bool CanConnectTo(Route<ValueT> other, bool directed)
    {
        return other is not null
            && (End.Equals(other.Start)
                || (!directed
                    && (Start.Equals(other.Start)
                        || Start.Equals(other.End)
                        || End.Equals(other.End))))
            && !Parallels(other)
            && !Intersects(other);
    }

    public bool Parallels(Route<ValueT> other)
    {
        return other is not null
            && ((Start.Equals(other.Start) && End.Equals(other.End))
                || (Start.Equals(other.End) && End.Equals(other.Start)));
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

    public bool Ordered(ValueT a, ValueT b)
    {
        var x = Array.IndexOf(nodes, a);
        var y = Array.IndexOf(nodes, b);
        return x != -1
            && y != -1
            && x < y;
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(Cost);

        var nodes = this.nodes.AsEnumerable();
        if (Start.CompareTo(End) > 0)
        {
            nodes = nodes.Reverse();
        }
        foreach (var node in nodes)
        {
            hash.Add(node);
        }

        return hash.ToHashCode();
    }
}