namespace Juniper.Logic;

public static class LogicConstructor
{
    public static IExpression<ItemT> Empty<ItemT>() =>
        new EmptyExpression<ItemT>();

    public static IExpression<ItemT> Expr<ItemT>(ItemT value) => 
        new ItemExpression<ItemT>(value);

    private static IEnumerable<IExpression<ItemT>> Exprs<ItemT>(IEnumerable<ItemT> items) => 
        items
            .Select(Expr)
            .ToArray();

    public static IExpression<ItemT> Not<ItemT>(IExpression<ItemT> expr) => 
        new NotExpression<ItemT>(expr);

    public static IExpression<ItemT> Not<ItemT>(ItemT item) => 
        Not(Expr(item));

    private static IExpression<ItemT> Create<ItemT>(IEnumerable<IExpression<ItemT>> exprs, Func<IExpression<ItemT>, IExpression<ItemT>, IExpression<ItemT>> combine)
    {
        if (exprs is null)
        {
            throw new ArgumentNullException(nameof(exprs));
        }

        IExpression<ItemT>? expr = null;
        var count = 0;
        foreach (var x in exprs)
        {
            if (expr is null)
            {
                expr = x;
            }
            else
            {
                expr = combine(expr, x);
            }
            ++count;
        }

        return expr ?? Empty<ItemT>();
    }

    public static IExpression<ItemT> And<ItemT>(IEnumerable<IExpression<ItemT>> exprs) =>
        Create(exprs, (left, right) => new AndExpression<ItemT>(left, right));

    public static IExpression<ItemT> And<ItemT>(params IExpression<ItemT>[] exprs) => 
        And((IEnumerable<IExpression<ItemT>>)exprs);

    public static IExpression<ItemT> And<ItemT>(IEnumerable<ItemT> items) => 
        And(Exprs(items));

    public static IExpression<ItemT> And<ItemT>(params ItemT[] items) => 
        And((IEnumerable<ItemT>)items);

    public static IExpression<ItemT> Or<ItemT>(IEnumerable<IExpression<ItemT>> exprs) =>
        Create(exprs, (left, right) => new OrExpression<ItemT>(left, right));

    public static IExpression<ItemT> Or<ItemT>(params IExpression<ItemT>[] exprs) => 
        Or((IEnumerable<IExpression<ItemT>>)exprs);

    public static IExpression<ItemT> Or<ItemT>(IEnumerable<ItemT> items) => 
        Or(Exprs(items));

    public static IExpression<ItemT> Or<ItemT>(params ItemT[] items) => 
        Or((IEnumerable<ItemT>)items);

    public static IExpression<ItemT> Xor<ItemT>(IEnumerable<IExpression<ItemT>> exprs) =>
        Create(exprs, (left, right) => new XorExpression<ItemT>(left, right));

    public static IExpression<ItemT> Xor<ItemT>(params IExpression<ItemT>[] exprs) =>
        Xor((IEnumerable<IExpression<ItemT>>)exprs);

    public static IExpression<ItemT> Xor<ItemT>(IEnumerable<ItemT> items) =>
        Xor(Exprs(items));

    public static IExpression<ItemT> Xor<ItemT>(params ItemT[] items) =>
        Xor((IEnumerable<ItemT>)items);
}