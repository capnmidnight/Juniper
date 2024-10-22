namespace System.Runtime.Serialization;

public static class SerializationInfoExt
{
    public static T? GetValue<T>(this SerializationInfo info, string name)
    {
        return (T?)info.GetValue(name, typeof(T));
    }

    public static List<T> GetList<T>(this SerializationInfo info, string name)
    {
        var list = new List<T>();
        var arr = info.GetValue<T[]>(name);
        if (arr is not null)
        {
            list.AddRange(arr);
        }
        return list;
    }

    public static void AddList<T>(this SerializationInfo info, string name, List<T> list)
    {
        info.AddValue(name, list?.ToArray());
    }

    public static T GetEnumFromString<T>(this SerializationInfo info, string name)
        where T : struct, Enum
    {
        if (info is null)
        {
            throw new ArgumentNullException(nameof(info));
        }

        var value = info.GetString(name);
        if (!string.IsNullOrEmpty(value)
            && Enum.TryParse(value, out T result))
        {
            return result;
        }
        else
        {
            return default;
        }
    }

    public static void SetEnumAsString<T>(this SerializationInfo info, string name, T value)
        where T : struct, Enum
    {
        info.AddValue(name, value.ToString());
    }

    public static bool MaybeAddValue<T>(this SerializationInfo info, string name, T? value)
        where T : class
    {
        if (value is not null)
        {
            info.AddValue(name, value);
            return true;
        }

        return false;
    }

    public static void CheckForType(this SerializationInfo info, string expected)
    {
        foreach (var field in info)
        {
            if (field.Name == "Type"
                && field.Value is string actual
                && actual != expected)
            {
                throw new SerializationException($"Input type `{actual}` does not match expected type `{expected}`.");
            }
        }
    }
}