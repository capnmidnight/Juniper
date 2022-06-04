namespace Juniper.Logic
{
    internal abstract class AbstractBinaryExpression<ItemT> :
        IExpression<ItemT>
    {
        private readonly string opName;

        protected IExpression<ItemT> Item1 { get; }

        protected IExpression<ItemT> Item2 { get; }

        protected AbstractBinaryExpression(string opName, IExpression<ItemT> item1, IExpression<ItemT> item2)
        {
            this.opName = opName;
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

        public bool HasNestedElements => Item1 is AbstractBinaryExpression<ItemT>
                    || Item2 is AbstractBinaryExpression<ItemT>;

        public IEnumerable<IExpression<ItemT>> GetExpressions()
        {
            yield return Item1;
            yield return Item2;
        }

        public override string ToString()
        {
            if (Item1 is EmptyExpression<ItemT>
                && Item2 is EmptyExpression<ItemT>)
            {
                return string.Empty;
            }
            else if (Item1 is EmptyExpression<ItemT>)
            {
                return Item2.ToString();
            }
            else if (Item2 is EmptyExpression<ItemT>)
            {
                return Item1.ToString();
            }
            else if (HasNestedElements)
            {
                return $"({Item1} {opName} {Item2})";
            }
            else
            {
                return $"{Item1} {opName} {Item2}";
            }
        }

        public abstract bool Evaluate(Func<ItemT, bool> evaluator);
    }
}
