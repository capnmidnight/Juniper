using System.Text;

namespace Juniper.IO;

public static class ISerializerExt
{
    public static byte[] Serialize<T, M>(this ISerializer<T, M> serializer, T value)
        where M : MediaType
    {
        if (serializer is null)
        {
            throw new ArgumentNullException(nameof(serializer));
        }

        using var mem = new MemoryStream();
        serializer.Serialize(mem, value);
        mem.Flush();

        return mem.ToArray();
    }

    public static void Serialize<T, M>(this ISerializer<T, M> serializer, FileInfo file, T value)
        where M : MediaType
    {
        if (serializer is null)
        {
            throw new ArgumentNullException(nameof(serializer));
        }

        if (file is null)
        {
            throw new ArgumentNullException(nameof(file));
        }

        using var stream = file.Open(FileMode.Create, FileAccess.Write, FileShare.Read);
        serializer.Serialize(stream, value);
    }

    public static void Serialize<T, M>(this ISerializer<T, M> serializer, string fileName, T value)
        where M : MediaType
    {
        if (fileName is null)
        {
            throw new ArgumentNullException(nameof(fileName));
        }

        if (fileName.Length == 0)
        {
            throw new ArgumentException("path must not be empty string", nameof(fileName));
        }

        serializer.Serialize(new FileInfo(fileName), value);
    }

    public static MemoryStream GetStream<T, M>(this ISerializer<T, M> serializer, T value)
        where M : MediaType
    {
        var stream = new MemoryStream();
        serializer.Serialize(stream, value);
        stream.Flush();
        stream.Position = 0;
        return stream;
    }

    public static string ToString<T, M>(this ISerializer<T, M> serializer, T value)
        where M : MediaType
    {
        if (serializer is null)
        {
            throw new ArgumentNullException(nameof(serializer));
        }

        return Encoding.UTF8.GetString(serializer.Serialize(value));
    }
}
