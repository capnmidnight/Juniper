namespace Juniper.Logic;

public static class Always
{
    public static bool True()
    {
        return true;
    }

    public static bool True<T>(T _)
    {
        return true;
    }

    public static bool True<T1, T2>(T1 _1, T2 _2)
    {
        return true;
    }

    public static bool True<T1, T2, T3>(T1 _1, T2 _2, T3 _3)
    {
        return true;
    }

    public static bool True<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4)
    {
        return true;
    }

    public static bool True<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
    {
        return true;
    }

    public static bool True<T1, T2, T3, T4, T5, T6>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6)
    {
        return true;
    }

    public static bool False()
    {
        return false;
    }

    public static bool False<T>(T _)
    {
        return false;
    }

    public static bool False<T1, T2>(T1 _1, T2 _2)
    {
        return false;
    }

    public static bool False<T1, T2, T3>(T1 _1, T2 _2, T3 _3)
    {
        return false;
    }

    public static bool False<T1, T2, T3, T4>(T1 _1, T2 _2, T3 _3, T4 _4)
    {
        return false;
    }

    public static bool False<T1, T2, T3, T4, T5>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5)
    {
        return false;
    }

    public static bool False<T1, T2, T3, T4, T5, T6>(T1 _1, T2 _2, T3 _3, T4 _4, T5 _5, T6 _6)
    {
        return false;
    }

    public static bool Not(bool value)
    {
        return !value;
    }

    public static Func<bool> Not(Func<bool> func)
    {
        return () => !func();
    }

    public static Func<T, bool> Not<T>(Func<T, bool> func)
    {
        return (arg) => !func(arg);
    }

    public static Func<T1, T2, bool> Not<T1, T2>(Func<T1, T2, bool> func)
    {
        return (arg1, arg2) => !func(arg1, arg2);
    }

    public static Func<T1, T2, T3, bool> Not<T1, T2, T3>(Func<T1, T2, T3, bool> func)
    {
        return (arg1, arg2, arg3) => !func(arg1, arg2, arg3);
    }

    public static Func<T1, T2, T3, T4, bool> Not<T1, T2, T3, T4>(Func<T1, T2, T3, T4, bool> func)
    {
        return (arg1, arg2, arg3, arg4) => !func(arg1, arg2, arg3, arg4);
    }

    public static Func<T1, T2, T3, T4, T5, bool> Not<T1, T2, T3, T4, T5>(Func<T1, T2, T3, T4, T5, bool> func)
    {
        return (arg1, arg2, arg3, arg4, arg5) => !func(arg1, arg2, arg3, arg4, arg5);
    }

    public static Func<T1, T2, T3, T4, T5, T6, bool> Not<T1, T2, T3, T4, T5, T6>(Func<T1, T2, T3, T4, T5, T6, bool> func)
    {
        return (arg1, arg2, arg3, arg4, arg5, arg6) => !func(arg1, arg2, arg3, arg4, arg5, arg6);
    }

    public static Func<T, bool> Eq<T>(T? arg1)
    {
        return arg2 => arg1?.Equals(arg2) == true;
    }

    public static T Identity<T>(T value)
    {
        return value;
    }

    public static bool Null<T>(T? value)
        where T : class
    {
        return value is null;
    }

    public static bool NotNull<T>(T? value)
        where T : class
    {
        return value is not null;
    }
}
