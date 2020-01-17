using System;
using System.Collections.Generic;

namespace Juniper.Logic
{
    abstract class AbstractBinaryExpression<ItemT> :
        IExpression<ItemT>
    {
        protected IExpression<ItemT> Item1 { get; }
        protected IExpression<ItemT> Item2 { get; }

        protected AbstractBinaryExpression(IExpression<ItemT> item1, IExpression<ItemT> item2)
        {
            Item1 = item1 ?? throw new ArgumentNullException(nameof(item1));
            Item2 = item2 ?? throw new ArgumentNullException(nameof(item2));
        }

        public IEnumerable<ItemT> GetItems()
        {
            foreach (var item in Item1.GetItems())
            {
                yield return item;
            }

            foreach (var item in Item2.GetItems())
            {
                yield return item;
            }
        }

        public IEnumerable<IExpression<ItemT>> GetExpressions()
        {
            yield return Item1;
            yield return Item2;
        }

        protected string ToString(string operatorToken)
        {
            if(Item1 is EmptyExpression<ItemT>
                && Item2 is EmptyExpression<ItemT>)
            {
                return string.Empty;
            }
            else if(Item1 is EmptyExpression<ItemT>)
            {
                return Item2.ToString();
            }
            else if(Item2 is EmptyExpression<ItemT>)
            {
                return Item1.ToString();
            }
            else
            {
                return $"({Item1} {operatorToken} {Item2})";
            }
        }

        public abstract bool Evaluate(ExpressionEvaluator<ItemT> evaluator);
    }
}
