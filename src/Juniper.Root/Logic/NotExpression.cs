using System.Collections.Generic;

namespace Juniper.Logic
{
    class NotExpression<ItemT> :
        AbstractUnaryExpression<ItemT, IExpression<ItemT>>
    {
        public NotExpression(IExpression<ItemT> expr)
            : base(expr)
        { }

        public override bool Evaluate(ExpressionEvaluator<ItemT> evaluator)
        {
            return !Value.Evaluate(evaluator);
        }

        public override IEnumerable<ItemT> GetItems()
        {
            foreach (var item in Value.GetItems())
            {
                yield return item;
            }
        }

        public override string ToString()
        {
            return $"!{Value}";
        }
    }
}