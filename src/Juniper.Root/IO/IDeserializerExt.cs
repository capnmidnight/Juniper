using Juniper.Progress;

using System.Net;
using System.Text;

namespace Juniper.IO
{

    public static class IDeserializerExt
    {
        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, FileInfo file, IProgress? prog = null)
            where M : MediaType
        {
            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            using var stream = file.OpenRead();
            return deserializer.Deserialize(stream, file.Length, prog);
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, Stream stream, long length, IProgress? prog = null)
            where M : MediaType
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

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, Stream stream, IProgress? prog = null)
            where M : MediaType
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

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, Stream stream, out ResultT? value, long length, IProgress? prog = null)
            where M : MediaType
        {
            using var progStream = new ProgressStream(stream, length, prog, false);
            return deserializer.TryDeserialize(progStream, out value);
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, Stream stream, out ResultT? value, IProgress? prog = null)
            where M : MediaType
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using var progStream = new ProgressStream(stream, stream.Length, prog, false);
            return deserializer.TryDeserialize(progStream, out value);
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, Stream stream, out ResultT? value)
            where M : MediaType
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
#pragma warning disable IDE0059 // Unnecessary assignment of a value
#pragma warning disable CS0168 // Variable is declared but never used
            catch (Exception exp)
#pragma warning restore CS0168 // Variable is declared but never used
#pragma warning restore IDE0059 // Unnecessary assignment of a value
            {
                value = default;
                return false;
            }
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, byte[] data, IProgress? prog = null)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, byte[] data)
            where M : MediaType
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

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, byte[] data, out ResultT? value, IProgress? prog = null)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, byte[] data, out ResultT? value)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            using var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value);
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, IReadOnlyCollection<byte> data, IProgress? prog = null)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.Deserialize(data.ToArray(), prog);
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, IReadOnlyCollection<byte> data)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.Deserialize(data.ToArray());
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, IReadOnlyCollection<byte> data, out ResultT? value, IProgress? prog = null)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.TryDeserialize(data.ToArray(), out value, prog);
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, IReadOnlyCollection<byte> data, out ResultT? value)
            where M : MediaType
        {
            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.TryDeserialize(data.ToArray(), out value);
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpWebResponse response, IProgress? prog = null)
            where M : MediaType
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

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpWebResponse response)
            where M : MediaType
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

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpWebResponse response, out ResultT? value, IProgress? prog = null)
            where M : MediaType
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

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpWebResponse response, out ResultT? value)
            where M : MediaType
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

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpListenerRequest request, IProgress? prog = null)
            where M : MediaType
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

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpListenerRequest request)
            where M : MediaType
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

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpListenerRequest request, out ResultT? value, IProgress? prog = null)
            where M : MediaType
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

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpListenerRequest request, out ResultT? value)
            where M : MediaType
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

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpResponseMessage response, IProgress? prog = null)
            where M : MediaType
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.Content.ReadAsStream();
            if (response.Content.Headers.ContentLength is not null)
            {
                return deserializer.Deserialize(stream, response.Content.Headers.ContentLength.Value, prog);
            }
            else
            {
                return deserializer.Deserialize(stream, prog);
            }
        }

        public static ResultT? Deserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpResponseMessage response)
            where M : MediaType
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.Content.ReadAsStream();
            return deserializer.Deserialize(stream);
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpResponseMessage response, out ResultT? value, IProgress? prog = null)
            where M : MediaType
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.Content.ReadAsStream();
            if (response.Content.Headers.ContentLength is not null)
            {
                return deserializer.TryDeserialize(stream, out value, response.Content.Headers.ContentLength.Value, prog);
            }
            else
            {
                return deserializer.TryDeserialize(stream, out value, prog);
            }
        }

        public static bool TryDeserialize<ResultT, M>(this IDeserializer<ResultT, M> deserializer, HttpResponseMessage response, out ResultT? value)
            where M : MediaType
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (response is null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            using var stream = response.Content.ReadAsStream();
            return deserializer.TryDeserialize(stream, out value);
        }

        public static ResultT? Parse<ResultT, M>(this IDeserializer<ResultT, M> deserializer, string text, IProgress? prog = null)
            where M : MediaType
        {
            if (text is null)
            {
                throw new ArgumentNullException(nameof(text));
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static bool TryParse<ResultT, M>(this IDeserializer<ResultT, M> deserializer, string text, out ResultT? value, IProgress? prog = null)
            where M : MediaType
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