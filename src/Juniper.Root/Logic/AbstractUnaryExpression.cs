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

        public bool HasNestedElements
        {
            get
            {
                return false;
            }
        }

        public IEnumerable<IExpression<ItemT>> GetExpressions()
        {
            yield return this;
        }

        public abstract IEnumerable<ItemT> GetItems();

        public abstract bool Evaluate(Func<ItemT, bool> evaluator);
    }
}
