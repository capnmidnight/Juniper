namespace Juniper.Logic;

public interface IExpression<ItemT>
{
    bool HasNestedElements { get; }

    bool Evaluate(Func<ItemT, bool> evaluator);

    IEnumerable<ItemT> GetItems();

    IEnumerable<IExpression<ItemT>> GetExpressions();
}
