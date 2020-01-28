using System;
using System.Collections.Generic;

namespace Juniper.Logic
{
    internal abstract class AbstractUnaryExpression<ItemT, ValueT> :
        IExpression<ItemT>
    {
        protected ValueT Value { get; }

        protected AbstractUnaryExpression(ValueT item)
        {
            Value = item;
        }

        public bool HasNestedElements => false;

        public IEnumerable<IExpression<ItemT>> GetExpressions()
        {
            yield return this;
        }

        public abstract IEnumerable<ItemT> GetItems();

        public abstract bool Evaluate(Func<ItemT, bool> evaluator);
    }
}
