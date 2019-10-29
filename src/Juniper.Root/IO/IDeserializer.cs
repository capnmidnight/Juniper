using System.IO;
using System.Net;
using System.Text;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer
    {
        MediaType ContentType { get; }

        T Deserialize<T>(Stream stream, IProgress prog);
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

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, Stream stream, out T value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, HttpWebResponse response, IProgress prog)
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.Deserialize<T>(stream, response.ContentLength, prog);
            }
        }

        public static T Deserialize<T>(this IDeserializer deserializer, HttpWebResponse response)
        {
            return deserializer.Deserialize<T>(response, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, HttpWebResponse response, out T value, IProgress prog)
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.TryDeserialize(stream, out value, response.ContentLength, prog);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, HttpWebResponse response, out T value)
        {
            return deserializer.TryDeserialize<T>(response, out value, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize<T>(stream, data.Length, prog);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data)
        {
            return deserializer.Deserialize<T>(data, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, byte[] data, out T value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, FileInfo file, IProgress prog)
        {
            return deserializer.Deserialize<T>(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), file.Length, prog);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, FileInfo file)
        {
            return deserializer.Deserialize<T>(file, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, FileInfo file, out T value, IProgress prog)
        {
            return deserializer.TryDeserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), out value, file.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, FileInfo file, out T value)
        {
            return deserializer.TryDeserialize(file, out value, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, string fileName, IProgress prog)
        {
            return deserializer.Deserialize<T>(new FileInfo(fileName), prog);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, string fileName)
        {
            return deserializer.Deserialize<T>(fileName, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, string fileName, out T value, IProgress prog)
        {
            return deserializer.TryDeserialize(new FileInfo(fileName), out value, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, string fileName, out T value)
        {
            return deserializer.TryDeserialize(fileName, out value, null);
        }

        public static T Parse<T>(this IDeserializer deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize<T>(stream, stream.Length, prog);
        }

        public static T Parse<T>(this IDeserializer deserializer, string text)
        {
            return deserializer.Parse<T>(text, null);
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static bool TryParse<T>(this IDeserializer deserializer, string text, out T value)
        {
            return deserializer.TryParse(text, out value, null);
        }
    }
}