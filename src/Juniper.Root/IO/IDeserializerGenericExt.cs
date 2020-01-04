using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Juniper.Progress;

namespace Juniper.IO
{

    public static class IDeserializerGenericExt
    {
        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, long length, IProgress prog = null)
        {
            var subProgs = prog.Split(2);
            if(prog != null)
            {
                stream = new ProgressStream(stream, length, subProgs[0]);
            }
            return deserializer.Deserialize(stream, subProgs[1]);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, IProgress prog = null)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, IProgress prog = null)
        {
            return deserializer.Deserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), file.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, IProgress prog = null)
        {
            return deserializer.Deserialize(new FileInfo(fileName), prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, IProgress prog = null)
        {
            return deserializer.Deserialize(data.ToArray(), prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, IProgress prog = null)
        {
            using var stream = response.GetResponseStream();
            return deserializer.Deserialize(stream, response.ContentLength, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, IProgress prog = null)
        {
            using var stream = request.InputStream;
            return deserializer.Deserialize(stream, request.ContentLength64, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, out ResultT value, IProgress prog = null)
        {
            using var stream = response.GetResponseStream();
            return deserializer.TryDeserialize(stream, out value, response.ContentLength, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, out ResultT value, IProgress prog = null)
        {
            using var stream = request.InputStream;
            return deserializer.TryDeserialize(stream, out value, request.ContentLength64, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, IProgress prog = null)
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

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, long length, IProgress prog = null)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value, IProgress prog = null)
        {
            if (data is null)
            {
                throw new System.ArgumentNullException(nameof(data));
            }

            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, out ResultT value, IProgress prog = null)
        {
            return deserializer.TryDeserialize(data?.ToArray(), out value, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, out ResultT value, IProgress prog = null)
        {
            return deserializer.TryDeserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), out value, file.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, out ResultT value, IProgress prog = null)
        {
            return deserializer.TryDeserialize(new FileInfo(fileName), out value, prog);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text, IProgress prog = null)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value, IProgress prog = null)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }
    }
}