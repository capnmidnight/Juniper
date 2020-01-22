using System;
using System.Collections.Generic;

namespace Juniper.Logic
{
    class EmptyExpression<ItemT> :
        IExpression<ItemT>
    {
        public EmptyExpression()
        { }

        public bool Evaluate(Func<ItemT, bool> _)
        {
            return true;
        }

        public bool HasNestedElements
        {
            get
            {
                return false;
            }
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