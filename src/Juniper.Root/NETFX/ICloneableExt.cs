namespace System;

public static class ICloneableExt
{
    public static T Copy<T>(this T value)
        where T : ICloneable
    {
        return (T)value.Clone();
    }
}