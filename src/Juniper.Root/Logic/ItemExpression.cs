namespace Juniper.Logic;

internal class ItemExpression<ItemT> :
    AbstractUnaryExpression<ItemT, ItemT>
{
    public ItemExpression(ItemT value)
        : base(value)
    { }

    public static implicit operator ItemExpression<ItemT>(ItemT value)
    {
        return new ItemExpression<ItemT>(value);
    }

    public static implicit operator ItemT(ItemExpression<ItemT> expr)
    {
        return expr.Value;
    }

    public override bool Evaluate(Func<ItemT, bool> evaluator)
    {
        if (evaluator is null)
        {
            throw new ArgumentNullException(nameof(evaluator));
        }

        return evaluator(Value);
    }

    public override IEnumerable<ItemT> GetItems()
    {
        yield return Value;
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }
}
