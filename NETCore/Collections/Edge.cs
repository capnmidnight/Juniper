namespace Juniper.Collections;

public record Edge<KeyT, ValueT>(KeyT From, KeyT To, ValueT Value);

public record RoutingEdge<KeyT> : Edge<KeyT, float>
{
    public RoutingEdge(KeyT from, KeyT to, float value = 1)
        : base(from, to, value)
    { }
}