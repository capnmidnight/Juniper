namespace Juniper.Logic
{
    public static class LogicConstructor
    {
        public static IExpression<ItemT> Empty<ItemT>()
        {
            return new EmptyExpression<ItemT>();
        }

        public static IExpression<ItemT> Expr<ItemT>(ItemT value)
        {
            return new ItemExpression<ItemT>(value);
        }

        private static IEnumerable<IExpression<ItemT>> Exprs<ItemT>(IEnumerable<ItemT> items)
        {
            return items
                .Select(Expr)
                .ToArray();
        }

        public static IExpression<ItemT> Not<ItemT>(IExpression<ItemT> expr)
        {
            return new NotExpression<ItemT>(expr);
        }

        public static IExpression<ItemT> Not<ItemT>(ItemT item)
        {
            return Not(Expr(item));
        }

        public static IExpression<ItemT> And<ItemT>(IEnumerable<IExpression<ItemT>> exprs)
        {
            if (exprs is null)
            {
                throw new System.ArgumentNullException(nameof(exprs));
            }

            IExpression<ItemT> expr = null;
            var count = 0;
            foreach (var x in exprs)
            {
                if (expr is null)
                {
                    expr = x;
                }
                else
                {
                    expr = new AndExpression<ItemT>(expr, x);
                }
                ++count;
            }

            return expr ?? Empty<ItemT>();
        }

        public static IExpression<ItemT> And<ItemT>(params IExpression<ItemT>[] exprs)
        {
            return And((IEnumerable<IExpression<ItemT>>)exprs);
        }

        public static IExpression<ItemT> And<ItemT>(IEnumerable<ItemT> items)
        {
            return And(Exprs(items));
        }

        public static IExpression<ItemT> And<ItemT>(params ItemT[] items)
        {
            return And((IEnumerable<ItemT>)items);
        }

        public static IExpression<ItemT> Or<ItemT>(IEnumerable<IExpression<ItemT>> exprs)
        {
            if (exprs is null)
            {
                throw new System.ArgumentNullException(nameof(exprs));
            }

            IExpression<ItemT> expr = null;
            var count = 0;
            foreach (var x in exprs)
            {
                if (expr is null)
                {
                    expr = x;
                }
                else
                {
                    expr = new OrExpression<ItemT>(expr, x);
                }
                ++count;
            }

            return expr ?? Empty<ItemT>();
        }

        public static IExpression<ItemT> Or<ItemT>(params IExpression<ItemT>[] exprs)
        {
            return Or((IEnumerable<IExpression<ItemT>>)exprs);
        }

        public static IExpression<ItemT> Or<ItemT>(IEnumerable<ItemT> items)
        {
            return Or(Exprs(items));
        }

        public static IExpression<ItemT> Or<ItemT>(params ItemT[] items)
        {
            return Or((IEnumerable<ItemT>)items);
        }

        public static IExpression<ItemT> Xor<ItemT>(IEnumerable<IExpression<ItemT>> exprs)
        {
            if (exprs is null)
            {
                throw new System.ArgumentNullException(nameof(exprs));
            }

            IExpression<ItemT> expr = null;
            var count = 0;
            foreach (var x in exprs)
            {
                if (expr is null)
                {
                    expr = x;
                }
                else
                {
                    expr = new XorExpression<ItemT>(expr, x);
                }
                ++count;
            }

            return expr ?? Empty<ItemT>();
        }

        public static IExpression<ItemT> Xor<ItemT>(params IExpression<ItemT>[] exprs)
        {
            return Xor((IEnumerable<IExpression<ItemT>>)exprs);
        }

        public static IExpression<ItemT> Xor<ItemT>(IEnumerable<ItemT> items)
        {
            return Xor(Exprs(items));
        }

        public static IExpression<ItemT> Xor<ItemT>(params ItemT[] items)
        {
            return Xor((IEnumerable<ItemT>)items);
        }
    }
}
