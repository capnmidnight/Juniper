using System;
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
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if(prog != null)
            {
                var subProgs = prog.Split(2);
                stream = new ProgressStream(stream, length, subProgs[0]);
                prog = subProgs[1];
            }

            return deserializer.Deserialize(stream, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, IProgress prog = null)
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
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            if (!file.Exists)
            {
                throw new FileNotFoundException("File not found!", file.FullName);
            }

            return deserializer.Deserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), file.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            return deserializer.Deserialize(new FileInfo(fileName.ValidateFileName()), prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.Deserialize(data.ToArray(), prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, IProgress prog = null)
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

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, IProgress prog = null)
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

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpWebResponse response, out ResultT value, IProgress prog = null)
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

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, HttpListenerRequest request, out ResultT value, IProgress prog = null)
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

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, IProgress prog = null)
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
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if(prog != null)
            {
                var subProgs = prog.Split(2);
                stream = new ProgressStream(stream, length, subProgs[0]);
                prog = subProgs[1];
            }

            return deserializer.TryDeserialize(stream, out value, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value, IProgress prog = null)
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
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, IReadOnlyCollection<byte> data, out ResultT value, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (data is null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            return deserializer.TryDeserialize(data.ToArray(), out value, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, out ResultT value, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new ArgumentNullException(nameof(deserializer));
            }

            if (file is null)
            {
                throw new ArgumentNullException(nameof(file));
            }

            return deserializer.TryDeserialize(file.Open(FileMode.Open, FileAccess.Read, FileShare.Read), out value, file.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, out ResultT value, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new System.ArgumentNullException(nameof(deserializer));
            }

            return deserializer.TryDeserialize(new FileInfo(fileName.ValidateFileName()), out value, prog);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new System.ArgumentNullException(nameof(deserializer));
            }

            if (text is null)
            {
                throw new System.ArgumentNullException(nameof(text));
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value, IProgress prog = null)
        {
            if (deserializer is null)
            {
                throw new System.ArgumentNullException(nameof(deserializer));
            }

            if (text is null)
            {
                throw new System.ArgumentNullException(nameof(text));
            }

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }
    }
}