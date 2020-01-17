using System;
using System.Collections.Generic;

namespace Juniper.Logic
{
    class EmptyExpression<ItemT> :
        IExpression<ItemT>
    {
        public EmptyExpression()
        { }

        public bool Evaluate(ExpressionEvaluator<ItemT> _)
        {
            return true;
        }

        public IEnumerable<ItemT> GetItems()
        {
            return Array.Empty<ItemT>();
        }

        public IEnumerable<IExpression<ItemT>> GetExpressions()
        {
            return Array.Empty<IExpression<ItemT>>();
        }

        public override string ToString()
        {
            return "Empty";
        }
    }
}
