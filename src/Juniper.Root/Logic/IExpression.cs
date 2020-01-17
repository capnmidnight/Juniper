using System.Collections.Generic;

namespace Juniper.Logic
{
    public delegate bool ExpressionEvaluator<ItemT>(ItemT item);

    public interface IExpression<ItemT>
    {
        bool Evaluate(ExpressionEvaluator<ItemT> evaluator);

        IEnumerable<ItemT> GetItems();

        IEnumerable<IExpression<ItemT>> GetExpressions();
    }
}
