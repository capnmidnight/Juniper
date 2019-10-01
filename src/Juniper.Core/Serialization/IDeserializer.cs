using System.IO;
using System.Text;
using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.Serialization
{
    public interface IDeserializer
    {
        T Deserialize<T>(Stream stream, IProgress prog);
    }

    public interface IDeserializer<T>
    {
        MediaType ReadContentType { get; }

        T Deserialize(Stream stream, IProgress prog);
    }

    public static class IDeserializerExt
    {
        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream)
        {
            return deserializer.Deserialize<T>(stream, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize<T>(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value, IProgress prog)
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

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data)
        {
            return deserializer.Deserialize<T>(data, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize<T>(stream, data.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static T Load<T>(this IDeserializer deserializer, FileInfo file)
        {
            return deserializer.Load<T>(file, null);
        }

        public static T Load<T>(this IDeserializer deserializer, FileInfo file, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize<T>(stream, file.Length, prog);
            }
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, FileInfo file, out T value)
        {
            return deserializer.TryLoad(file, out value, null);
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, FileInfo file, out T value, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, prog);
            }
        }

        public static T Load<T>(this IDeserializer deserializer, string fileName)
        {
            return deserializer.Load<T>(fileName, null);
        }

        public static T Load<T>(this IDeserializer deserializer, string fileName, IProgress prog)
        {
            return deserializer.Load<T>(new FileInfo(fileName), prog);
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, string fileName, out T value)
        {
            return deserializer.TryLoad(fileName, out value, null);
        }

        public static bool TryLoad<T>(this IDeserializer deserializer, string fileName, out T value, IProgress prog)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, prog);
        }

        public static T Parse<T>(this IDeserializer deserializer, string text)
        {
            return deserializer.Parse<T>(text, null);
        }

        public static T Parse<T>(this IDeserializer deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize<T>(stream, stream.Length, prog);
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value)
        {
            return deserializer.TryParse(text, out value, null);
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, Stream stream)
        {
            return deserializer.Deserialize(stream, null);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value, IProgress prog)
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

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data)
        {
            return deserializer.Deserialize(data, null);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static T Load<T>(this IDeserializer<T> deserializer, FileInfo file)
        {
            return deserializer.Load(file, null);
        }

        public static T Load<T>(this IDeserializer<T> deserializer, FileInfo file, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize(stream, file.Length, prog);
            }
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, FileInfo file, out T value)
        {
            return deserializer.TryLoad(file, out value, null);
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, FileInfo file, out T value, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, prog);
            }
        }

        public static T Load<T>(this IDeserializer<T> deserializer, string fileName)
        {
            return deserializer.Load(fileName, null);
        }

        public static T Load<T>(this IDeserializer<T> deserializer, string fileName, IProgress prog)
        {
            return deserializer.Load(new FileInfo(fileName), prog);
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, string fileName, out T value)
        {
            return deserializer.TryLoad(fileName, out value, null);
        }

        public static bool TryLoad<T>(this IDeserializer<T> deserializer, string fileName, out T value, IProgress prog)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, prog);
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text)
        {
            return deserializer.Parse(text, null);
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value)
        {
            return deserializer.TryParse(text, out value, null);
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }
    }
}