using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer
    {
        T Deserialize<T>(Stream stream, IProgress prog);
    }

    public interface IDeserializer<out T>
    {
        T Deserialize(Stream stream, IProgress prog);
    }

    public static class IDeserializerExt
    {
        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, Stream stream, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize<T>(stream, prog));
        }

        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream)
        {
            return deserializer.Deserialize<T>(stream, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, Stream stream)
        {
            return deserializer.DeserializeAsync<T>(stream, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize<T>(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, Stream stream, long length, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize<T>(stream, length, prog));
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

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, HttpWebResponse response, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize<T>(response, prog));
        }

        public static T Deserialize<T>(this IDeserializer deserializer, HttpWebResponse response)
        {
            return deserializer.Deserialize<T>(response, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, HttpWebResponse response)
        {
            return deserializer.DeserializeAsync<T>(response, null);
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

        public static T Deserialize<T>(this IDeserializer<T> deserializer, HttpWebResponse response, IProgress prog)
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.Deserialize(stream, response.ContentLength, prog);
            }
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize<T>(stream, data.Length, prog);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, byte[] data, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize<T>(data, prog));
        }

        public static T Deserialize<T>(this IDeserializer deserializer, byte[] data)
        {
            return deserializer.Deserialize<T>(data, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, byte[] data)
        {
            return deserializer.DeserializeAsync<T>(data, null);
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
            return deserializer.Deserialize<T>(file.OpenRead(), file.Length, prog);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, FileInfo file, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize<T>(file, prog));
        }

        public static T Deserialize<T>(this IDeserializer deserializer, FileInfo file)
        {
            return deserializer.Deserialize<T>(file, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, FileInfo file)
        {
            return deserializer.DeserializeAsync<T>(file, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, FileInfo file, out T value, IProgress prog)
        {
            return deserializer.TryDeserialize(file.OpenRead(), out value, file.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer deserializer, FileInfo file, out T value)
        {
            return deserializer.TryDeserialize(file, out value, null);
        }

        public static T Deserialize<T>(this IDeserializer deserializer, string fileName, IProgress prog)
        {
            return deserializer.Deserialize<T>(new FileInfo(fileName), prog);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, string fileName, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize<T>(fileName, prog));
        }

        public static T Deserialize<T>(this IDeserializer deserializer, string fileName)
        {
            return deserializer.Deserialize<T>(fileName, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer deserializer, string fileName)
        {
            return deserializer.DeserializeAsync<T>(fileName, null);
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

        public static Task<T> ParseAsync<T>(this IDeserializer deserializer, string text, IProgress prog)
        {
            return Task.Run(() => deserializer.Parse<T>(text, prog));
        }

        public static T Parse<T>(this IDeserializer deserializer, string text)
        {
            return deserializer.Parse<T>(text, null);
        }

        public static Task<T> ParseAsync<T>(this IDeserializer deserializer, string text)
        {
            return deserializer.ParseAsync<T>(text, null);
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

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, Stream stream, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize(stream, prog));
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, Stream stream)
        {
            return deserializer.Deserialize(stream, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, Stream stream)
        {
            return deserializer.DeserializeAsync(stream, null);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, Stream stream, long length, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize(stream, length, prog));
        }
        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, HttpWebResponse response, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize(response, prog));
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, HttpWebResponse response)
        {
            return deserializer.Deserialize(response, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, HttpWebResponse response)
        {
            return deserializer.DeserializeAsync(response, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, HttpWebResponse response, out T value, IProgress prog)
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.TryDeserialize(stream, out value, response.ContentLength, prog);
            }
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, HttpWebResponse response, out T value)
        {
            return deserializer.TryDeserialize(response, out value, null);
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

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, Stream stream, out T value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, byte[] data, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize(data, prog));
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, byte[] data)
        {
            return deserializer.Deserialize(data, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, byte[] data)
        {
            return deserializer.DeserializeAsync(data, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, byte[] data, out T value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, FileInfo file, IProgress prog)
        {
            return deserializer.Deserialize(file.OpenRead(), file.Length, prog);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, FileInfo file, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize(file, prog));
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, FileInfo file)
        {
            return deserializer.Deserialize(file, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, FileInfo file)
        {
            return deserializer.DeserializeAsync(file, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, FileInfo file, out T value, IProgress prog)
        {
            return deserializer.TryDeserialize(file.OpenRead(), out value, file.Length, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, FileInfo file, out T value)
        {
            return deserializer.TryDeserialize(file, out value, null);
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, string fileName, IProgress prog)
        {
            return deserializer.Deserialize(new FileInfo(fileName), prog);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, string fileName, IProgress prog)
        {
            return Task.Run(() => deserializer.Deserialize(fileName, prog));
        }

        public static T Deserialize<T>(this IDeserializer<T> deserializer, string fileName)
        {
            return deserializer.Deserialize(fileName, null);
        }

        public static Task<T> DeserializeAsync<T>(this IDeserializer<T> deserializer, string fileName)
        {
            return deserializer.DeserializeAsync(fileName, null);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, string fileName, out T value, IProgress prog)
        {
            return deserializer.TryDeserialize(new FileInfo(fileName), out value, prog);
        }

        public static bool TryDeserialize<T>(this IDeserializer<T> deserializer, string fileName, out T value)
        {
            return deserializer.TryDeserialize(fileName, out value, null);
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static Task<T> ParseAsync<T>(this IDeserializer<T> deserializer, string text, IProgress prog)
        {
            return Task.Run(() => deserializer.Parse(text, prog));
        }

        public static T Parse<T>(this IDeserializer<T> deserializer, string text)
        {
            return deserializer.Parse(text, null);
        }

        public static Task<T> ParseAsync<T>(this IDeserializer<T> deserializer, string text)
        {
            return deserializer.ParseAsync(text, null);
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static bool TryParse<T>(this IDeserializer<T> deserializer, string text, out T value)
        {
            return deserializer.TryParse(text, out value, null);
        }
    }
}