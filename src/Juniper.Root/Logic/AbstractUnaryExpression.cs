using System;
using System.Collections.Generic;

namespace Juniper.Logic
{
    abstract class AbstractUnaryExpression<ItemT, ValueT> :
        IExpression<ItemT>
    {
        protected ValueT Value { get; }

        protected AbstractUnaryExpression(ValueT item)
        {
            Value = item ?? throw new ArgumentNullException(nameof(item));
        }

        public abstract IEnumerable<ItemT> GetItems();

        public IEnumerable<IExpression<ItemT>> GetExpressions()
        {
            yield return this;
        }

        public abstract bool Evaluate(ExpressionEvaluator<ItemT> evaluator);
    }
}
