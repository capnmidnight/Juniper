using Juniper.Progress;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Juniper.IO
{

    public static class IDeserializerExt
    {
        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, IProgress prog)
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var stream = file.OpenRead();
            return deserializer.Deserialize(stream, file.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, long length, IProgress prog)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var progStream = new ProgressStream(stream, length, prog, false);
            return deserializer.Deserialize(progStream);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, IProgress prog)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var progStream = new ProgressStream(stream, stream.Length, prog, false);
            return deserializer.Deserialize(progStream);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, long length, IProgress prog)
        {
            using var progStream = new ProgressStream(stream, length, prog, false);
            return deserializer.TryDeserialize(progStream, out value);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, IProgress prog)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var progStream = new ProgressStream(stream, stream.Length, prog, false);
            return deserializer.TryDeserialize(progStream, out value);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

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

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, IProgress prog)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value, IProgress prog)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, IProgress prog)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.Deserialize(data.ToArray(), prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.Deserialize(data.ToArray());
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, out ResultT value, IProgress prog)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.TryDeserialize(data.ToArray(), out value, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, out ResultT value)
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.TryDeserialize(data.ToArray(), out value);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, IProgress prog)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.GetResponseStream();
            return deserializer.Deserialize(stream, response.ContentLength, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.GetResponseStream();
            return deserializer.Deserialize(stream);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, out ResultT value, IProgress prog)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.GetResponseStream();
            return deserializer.TryDeserialize(stream, out value, response.ContentLength, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, out ResultT value)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.GetResponseStream();
            return deserializer.TryDeserialize(stream, out value);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, IProgress prog)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var stream = request.InputStream;
            return deserializer.Deserialize(stream, request.ContentLength64, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var stream = request.InputStream;
            return deserializer.Deserialize(stream);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, out ResultT value, IProgress prog)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var stream = request.InputStream;
            return deserializer.TryDeserialize(stream, out value, request.ContentLength64, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, out ResultT value)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (request is null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            using var stream = request.InputStream;
            return deserializer.TryDeserialize(stream, out value);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text, IProgress prog = null)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value, IProgress prog = null)
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }
    }
}