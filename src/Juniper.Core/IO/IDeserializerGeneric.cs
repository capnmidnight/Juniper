using System.IO;
using System.Net;
using System.Text;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer<out ResultT, out MediaTypeT>
        where MediaTypeT : MediaType
    {
        MediaTypeT ContentType { get; }

        ResultT Deserialize(Stream stream, IProgress prog);
    }

    public static class IDeserializerGenericExt
    {
        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, Stream stream)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(stream, null);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, Stream stream, long length, IProgress prog)
            where MediaTypeT : MediaType
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, HttpWebResponse response, IProgress prog)
            where MediaTypeT : MediaType
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.Deserialize(stream, response.ContentLength, prog);
            }
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, HttpWebResponse response)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(response, null);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, HttpWebResponse response, out ResultT value, IProgress prog)
            where MediaTypeT : MediaType
        {
            using (var stream = response.GetResponseStream())
            {
                return deserializer.TryDeserialize(stream, out value, response.ContentLength, prog);
            }
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, HttpWebResponse response, out ResultT value)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(response, out value, null);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, Stream stream, out ResultT value, IProgress prog)
            where MediaTypeT : MediaType
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

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, Stream stream, out ResultT value)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, Stream stream, out ResultT value, long length, IProgress prog)
            where MediaTypeT : MediaType
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, byte[] data, IProgress prog)
            where MediaTypeT : MediaType
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, byte[] data)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(data, null);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, byte[] data, out ResultT value, IProgress prog)
            where MediaTypeT : MediaType
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, byte[] data, out ResultT value)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, FileInfo file, IProgress prog)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), file.Length, prog);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, FileInfo file)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(file, null);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, FileInfo file, out ResultT value, IProgress prog)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), out value, file.Length, prog);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, FileInfo file, out ResultT value)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(file, out value, null);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string fileName, IProgress prog)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(new FileInfo(fileName), prog);
        }

        public static ResultT Deserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string fileName)
            where MediaTypeT : MediaType
        {
            return deserializer.Deserialize(fileName, null);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string fileName, out ResultT value, IProgress prog)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(new FileInfo(fileName), out value, prog);
        }

        public static bool TryDeserialize<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string fileName, out ResultT value)
            where MediaTypeT : MediaType
        {
            return deserializer.TryDeserialize(fileName, out value, null);
        }

        public static ResultT Parse<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string text, IProgress prog)
            where MediaTypeT : MediaType
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static ResultT Parse<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string text)
            where MediaTypeT : MediaType
        {
            return deserializer.Parse(text, null);
        }

        public static bool TryParse<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string text, out ResultT value, IProgress prog)
            where MediaTypeT : MediaType
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static bool TryParse<ResultT, MediaTypeT>(this IDeserializer<ResultT, MediaTypeT> deserializer, string text, out ResultT value)
            where MediaTypeT : MediaType
        {
            return deserializer.TryParse(text, out value, null);
        }
    }
}