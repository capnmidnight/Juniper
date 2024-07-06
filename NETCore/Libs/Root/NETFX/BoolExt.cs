namespace System;

public static class BoolExt
{
    public static string ToYesNo(this bool value)
    {
        return value ? "Yes" : "No";
    }
    public static string ToYesNo(this bool value, string label)
    {
        if (value)
        {
            return label;
        }
        else
        {
            return "Not " + label;
        }
    }
}
