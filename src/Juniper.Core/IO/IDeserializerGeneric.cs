using System.IO;
using System.Net;
using System.Text;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer<out ResultT>
    {
        ResultT Deserialize(Stream stream, IProgress prog);
    }

    public static class IDeserializerGenericExt
    {
        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream)
        {
            return deserializer.Deserialize(stream, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, IProgress prog)
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.Deserialize(stream, response.ContentLength, prog);
            }
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response)
        {
            return deserializer.Deserialize(response, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, out ResultT value, IProgress prog)
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.TryDeserialize(stream, out value, response.ContentLength, prog);
            }
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, out ResultT value)
        {
            return deserializer.TryDeserialize(response, out value, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, IProgress prog)
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

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data)
        {
            return deserializer.Deserialize(data, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, IProgress prog)
        {
            return deserializer.Deserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), file.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file)
        {
            return deserializer.Deserialize(file, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, out ResultT value, IProgress prog)
        {
            return deserializer.TryDeserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), out value, file.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, out ResultT value)
        {
            return deserializer.TryDeserialize(file, out value, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, IProgress prog)
        {
            return deserializer.Deserialize(new FileInfo(fileName), prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName)
        {
            return deserializer.Deserialize(fileName, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, out ResultT value, IProgress prog)
        {
            return deserializer.TryDeserialize(new FileInfo(fileName), out value, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, out ResultT value)
        {
            return deserializer.TryDeserialize(fileName, out value, null);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text)
        {
            return deserializer.Parse(text, null);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value)
        {
            return deserializer.TryParse(text, out value, null);
        }
    }
}