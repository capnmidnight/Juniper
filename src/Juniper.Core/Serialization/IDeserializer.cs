using System.IO;
using System.Text;

using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.Serialization
{
    public interface IDeserializer
    {
        T Deserialize<T>(Stream stream);
    }

    public interface IDeserializer<T>
    {
        T Deserialize(Stream stream);
    }

    public static class IDeserializerExt
    {
        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream, long length, IProgress progress)
        {
            return deserializer.Deserialize<T>(new ProgressStream(stream, length, progress));
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value)
        {
            try
            {
                value = deserializer.Deserialize<T>(stream);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                value = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value, long length, IProgress progress)
        {
            return deserializer.TryDeserialize(new ProgressStream(stream, length, progress), out value);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize<T>(stream);
            }
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data, IProgress progress)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize<T>(stream, data.Length, progress);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.TryDeserialize<T>(stream, out value);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value, IProgress progress)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.TryDeserialize<T>(stream, out value, data.Length, progress);
            }
        }

        public static T Load<T>(this IDeserializer deserializer, FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize<T>(stream);
            }
        }

        public static T Load<T>(this IDeserializer deserializer, FileInfo file, IProgress progress)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize<T>(stream, file.Length, progress);
            }
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, FileInfo file, out T value)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value);
            }
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, FileInfo file, out T value, IProgress progress)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, progress);
            }
        }

        public static T Load<T>(this IDeserializer deserializer, string fileName)
        {
            return deserializer.Load<T>(new FileInfo(fileName));
        }

        public static T Load<T>(this IDeserializer deserializer, string fileName, IProgress progress)
        {
            return deserializer.Load<T>(new FileInfo(fileName), progress);
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, string fileName, out T value)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value);
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, string fileName, out T value, IProgress progress)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, progress);
        }

        public static T Parse<T>(this IDeserializer deserializer, string text)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.Deserialize<T>(stream);
            }
        }

        public static T Parse<T>(this IDeserializer deserializer, string text, IProgress progress)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.Deserialize<T>(stream, stream.Length, progress);
            }
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.TryDeserialize(stream, out value);
            }
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value, IProgress progress)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.TryDeserialize(stream, out value, stream.Length, progress);
            }
        }

        public static IDeserializer<T> Specialize<T>(this IDeserializer deserializer)
        {
            return new SpecializedDeserializer<T>(deserializer);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, Stream stream, long length, IProgress progress)
        {
            return deserializer.Deserialize(new ProgressStream(stream, length, progress));
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value)
        {
            try
            {
                value = deserializer.Deserialize(stream);
                return true;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch
            {
                value = default;
                return false;
            }
#pragma warning restore CA1031 // Do not catch general exception types
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value, long length, IProgress progress)
        {
            return deserializer.TryDeserialize(new ProgressStream(stream, length, progress), out value);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize(stream);
            }
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data, IProgress progress)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize(stream, data.Length, progress);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.TryDeserialize(stream, out value);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value, IProgress progress)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.TryDeserialize(stream, out value, data.Length, progress);
            }
        }

        public static T Load<T>(this IDeserializer<T> deserializer, FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize(stream);
            }
        }

        public static T Load<T>(this IDeserializer<T> deserializer, FileInfo file, IProgress progress)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize(stream, file.Length, progress);
            }
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, FileInfo file, out T value)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value);
            }
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, FileInfo file, out T value, IProgress progress)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, progress);
            }
        }

        public static T Load<T>(this IDeserializer<T> deserializer, string fileName)
        {
            return deserializer.Load(new FileInfo(fileName));
        }

        public static T Load<T>(this IDeserializer<T> deserializer, string fileName, IProgress progress)
        {
            return deserializer.Load(new FileInfo(fileName), progress);
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, string fileName, out T value)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value);
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, string fileName, out T value, IProgress progress)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, progress);
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.Deserialize(stream);
            }
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text, IProgress progress)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.Deserialize(stream, stream.Length, progress);
            }
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.TryDeserialize(stream, out value);
            }
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value, IProgress progress)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.TryDeserialize(stream, out value, stream.Length, progress);
            }
        }
    }
}