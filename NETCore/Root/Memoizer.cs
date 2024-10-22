namespace Juniper;

public class Memoizer
{
    private readonly Dictionary<Type, Dictionary<object, object>> objects = [];

    public void Clear()
    {
        objects.Clear();
    }

    internal V CreateInternal<V>(object key, Func<V> create)
        where V : class
    {
        var type = typeof(V);
        if (!objects.TryGetValue(type, out var ofType))
        {
            objects[type] = ofType = [];
        }

        if (!ofType.TryGetValue(key, out var value))
        {
            ofType[key] = value = create();
        }

        return (V)value;
    }
}

public static class MemoizerExt
{
    public static V Memo<V>(this object key, Memoizer? memo, Func<V> create)
        where V : class
    {
        if (memo is null)
        {
            return create();
        }

        return memo.CreateInternal(key, create);
    }
}