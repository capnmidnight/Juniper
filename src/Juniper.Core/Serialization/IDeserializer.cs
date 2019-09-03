using System.IO;
using System.Text;
using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.Serialization
{
    public interface IDeserializer
    {
        MediaType ContentType { get; }

        T Deserialize<T>(Stream stream, IProgress prog = null);
    }

    public interface IDeserializer<T>
    {
        MediaType ContentType { get; }

        T Deserialize(Stream stream, IProgress prog = null);
    }

    public static class IDeserializerExt
    {
        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize<T>(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value, IProgress prog = null)
        {
            try
            {
                value = deserializer.Deserialize<T>(stream, prog);
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

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data, IProgress prog = null)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize<T>(stream, data.Length, prog);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value, IProgress prog = null)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.TryDeserialize<T>(stream, out value, data.Length, prog);
            }
        }

        public static T Load<T>(this IDeserializer deserializer, FileInfo file, IProgress prog = null)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize<T>(stream, file.Length, prog);
            }
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, FileInfo file, out T value, IProgress prog = null)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, prog);
            }
        }

        public static T Load<T>(this IDeserializer deserializer, string fileName, IProgress prog = null)
        {
            return deserializer.Load<T>(new FileInfo(fileName), prog);
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, string fileName, out T value, IProgress prog = null)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, prog);
        }

        public static T Parse<T>(this IDeserializer deserializer, string text, IProgress prog = null)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.Deserialize<T>(stream, stream.Length, prog);
            }
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value, IProgress prog = null)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
            }
        }

        public static IDeserializer<T> Specialize<T>(this IDeserializer deserializer)
        {
            return new SpecializedDeserializer<T>(deserializer);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value, IProgress prog = null)
        {
            try
            {
                value = deserializer.Deserialize(stream, prog);
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

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data, IProgress prog = null)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.Deserialize(stream, data.Length, prog);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value, IProgress prog = null)
        {
            using (var stream = new MemoryStream(data))
            {
                return deserializer.TryDeserialize(stream, out value, data.Length, prog);
            }
        }

        public static T Load<T>(this IDeserializer<T> deserializer, FileInfo file, IProgress prog = null)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize(stream, file.Length, prog);
            }
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, FileInfo file, out T value, IProgress prog = null)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, prog);
            }
        }

        public static T Load<T>(this IDeserializer<T> deserializer, string fileName, IProgress prog = null)
        {
            return deserializer.Load(new FileInfo(fileName), prog);
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, string fileName, out T value, IProgress prog = null)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, prog);
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text, IProgress prog = null)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.Deserialize(stream, stream.Length, prog);
            }
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value, IProgress prog = null)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(text)))
            {
                return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
            }
        }
    }
}