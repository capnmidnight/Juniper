using System;

namespace Juniper.Logic
{
    class OrExpression<ItemT> :
        AbstractBinaryExpression<ItemT>
    {
        public OrExpression(IExpression<ItemT> item1, IExpression<ItemT> item2)
            : base("OR", item1, item2)
        { }

        public override bool Evaluate(Func<ItemT, bool> evaluator)
        {
            if (evaluator is null)
            {
                throw new ArgumentNullException(nameof(evaluator));
            }

            return Item1.Evaluate(evaluator) || Item2.Evaluate(evaluator);
        }
    }
}
