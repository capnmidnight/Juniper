namespace Juniper.Collections;

public class Edge<KeyT, ValueT>
{
    public KeyT From { get; }
    public KeyT To { get; }
    public ValueT Value { get; }

    public Edge(KeyT from, KeyT to, ValueT value)
    {
        From = from;
        To = to;
        Value = value;
    }
}

public class RoutingEdge<KeyT> : Edge<KeyT, float>
{
    public RoutingEdge(KeyT from, KeyT to, float value = 1)
        : base(from, to, value)
    { }
}