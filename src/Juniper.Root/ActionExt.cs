namespace System
{
    public static class ActionExt
    {
        public static Action Join(this Action act1, Action act2, params Action[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return () =>
            {
                foreach (var act in actions)
                {
                    act();
                }
            };
        }

        public static Action<T> Join<T>(this Action<T> act1, Action<T> act2, params Action<T>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (v) =>
            {
                foreach (var act in actions)
                {
                    act(v);
                }
            };
        }

        public static Action<T1, T2> Join<T1, T2>(this Action<T1, T2> act1, Action<T1, T2> act2, params Action<T1, T2>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b) =>
            {
                foreach (var act in actions)
                {
                    act(a, b);
                }
            };
        }

        public static Action<T1, T2, T3> Join<T1, T2, T3>(this Action<T1, T2, T3> act1, Action<T1, T2, T3> act2, params Action<T1, T2, T3>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c);
                }
            };
        }

        public static Action<T1, T2, T3, T4> Join<T1, T2, T3, T4>(this Action<T1, T2, T3, T4> act1, Action<T1, T2, T3, T4> act2, params Action<T1, T2, T3, T4>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5> Join<T1, T2, T3, T4, T5>(this Action<T1, T2, T3, T4, T5> act1, Action<T1, T2, T3, T4, T5> act2, params Action<T1, T2, T3, T4, T5>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6> Join<T1, T2, T3, T4, T5, T6>(this Action<T1, T2, T3, T4, T5, T6> act1, Action<T1, T2, T3, T4, T5, T6> act2, params Action<T1, T2, T3, T4, T5, T6>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7> Join<T1, T2, T3, T4, T5, T6, T7>(this Action<T1, T2, T3, T4, T5, T6, T7> act1, Action<T1, T2, T3, T4, T5, T6, T7> act2, params Action<T1, T2, T3, T4, T5, T6, T7>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8> Join<T1, T2, T3, T4, T5, T6, T7, T8>(this Action<T1, T2, T3, T4, T5, T6, T7, T8> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j, k) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j, k);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j, k, l) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j, k, l);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j, k, l, m) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j, k, l, m);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j, k, l, m, n) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j, k, l, m, n);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o);
                }
            };
        }

        public static Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> Join<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(this Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> act1, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> act2, params Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>[] actRest)
        {
            var actions = actRest.Prepend(act2).Prepend(act1).Where(act => act is not null);
            return (a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p) =>
            {
                foreach (var act in actions)
                {
                    act(a, b, c, d, e, f, g, h, i, j, k, l, m, n, o, p);
                }
            };
        }
    }
}
