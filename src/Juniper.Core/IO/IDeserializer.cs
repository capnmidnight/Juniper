using System.IO;
using System.Text;
using System.Threading.Tasks;
using Juniper.HTTP;
using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer
    {
        ResultT Deserialize<ResultT>(Stream stream, IProgress prog);
    }

    public interface IDeserializer<ResultT>
    {
        ResultT Deserialize(Stream stream, IProgress prog);
    }

    public static class IDeserializerExt
    {
        public static ResultT Deserialize<ResultT>(this IDeserializer deserializer, Stream stream)
        {
            return deserializer.Deserialize<ResultT>(stream, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize<ResultT>(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer deserializer, Stream stream, out ResultT value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer deserializer, Stream stream, out ResultT value, IProgress prog)
        {
            try
            {
                value = deserializer.Deserialize<ResultT>(stream, prog);
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

        public static bool TryDeserialize<ResultT>(this IDeserializer deserializer, Stream stream, out ResultT value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer deserializer, byte[] data)
        {
            return deserializer.Deserialize<ResultT>(data, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize<ResultT>(stream, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer deserializer, byte[] data, out ResultT value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer deserializer, byte[] data, out ResultT value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static ResultT Load<ResultT>(this IDeserializer deserializer, FileInfo file)
        {
            return deserializer.Load<ResultT>(file, null);
        }

        public static ResultT Load<ResultT>(this IDeserializer deserializer, FileInfo file, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize<ResultT>(stream, file.Length, prog);
            }
        }

        public static bool TryLoad<ResultT>(this IDeserializer deserializer, FileInfo file, out ResultT value)
        {
            return deserializer.TryLoad(file, out value, null);
        }

        public static bool TryLoad<ResultT>(this IDeserializer deserializer, FileInfo file, out ResultT value, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, prog);
            }
        }

        public static ResultT Load<ResultT>(this IDeserializer deserializer, string fileName)
        {
            return deserializer.Load<ResultT>(fileName, null);
        }

        public static ResultT Load<ResultT>(this IDeserializer deserializer, string fileName, IProgress prog)
        {
            return deserializer.Load<ResultT>(new FileInfo(fileName), prog);
        }

        public static bool TryLoad<ResultT>(this IDeserializer deserializer, string fileName, out ResultT value)
        {
            return deserializer.TryLoad(fileName, out value, null);
        }

        public static bool TryLoad<ResultT>(this IDeserializer deserializer, string fileName, out ResultT value, IProgress prog)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, prog);
        }

        public static ResultT Parse<ResultT>(this IDeserializer deserializer, string text)
        {
            return deserializer.Parse<ResultT>(text, null);
        }

        public static ResultT Parse<ResultT>(this IDeserializer deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize<ResultT>(stream, stream.Length, prog);
        }

        public static bool TryParse<ResultT>(this IDeserializer deserializer, string text, out ResultT value)
        {
            return deserializer.TryParse(text, out value, null);
        }

        public static bool TryParse<ResultT>(this IDeserializer deserializer, string text, out ResultT value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream)
        {
            return deserializer.Deserialize(stream, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.Deserialize(new ProgressStream(stream, length, subProgs[0]), subProgs[1]);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value)
        {
            return deserializer.TryDeserialize(stream, out value, null);
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

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, Stream stream, out ResultT value, long length, IProgress prog)
        {
            var subProgs = prog.Split(2);
            return deserializer.TryDeserialize(new ProgressStream(stream, length, subProgs[0]), out value, subProgs[1]);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data)
        {
            return deserializer.Deserialize(data, null);
        }

        public static ResultT Deserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.Deserialize(stream, data.Length, prog);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value)
        {
            return deserializer.TryDeserialize(data, out value, null);
        }

        public static bool TryDeserialize<ResultT>(this IDeserializer<ResultT> deserializer, byte[] data, out ResultT value, IProgress prog)
        {
            var stream = new MemoryStream(data);
            return deserializer.TryDeserialize(stream, out value, data.Length, prog);
        }

        public static ResultT Load<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file)
        {
            return deserializer.Load(file, null);
        }

        public static ResultT Load<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.Deserialize(stream, file.Length, prog);
            }
        }

        public static bool TryLoad<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, out ResultT value)
        {
            return deserializer.TryLoad(file, out value, null);
        }

        public static bool TryLoad<ResultT>(this IDeserializer<ResultT> deserializer, FileInfo file, out ResultT value, IProgress prog)
        {
            using (var stream = file.OpenRead())
            {
                return deserializer.TryDeserialize(stream, out value, file.Length, prog);
            }
        }

        public static ResultT Load<ResultT>(this IDeserializer<ResultT> deserializer, string fileName)
        {
            return deserializer.Load(fileName, null);
        }

        public static ResultT Load<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, IProgress prog)
        {
            return deserializer.Load(new FileInfo(fileName), prog);
        }

        public static bool TryLoad<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, out ResultT value)
        {
            return deserializer.TryLoad(fileName, out value, null);
        }

        public static bool TryLoad<ResultT>(this IDeserializer<ResultT> deserializer, string fileName, out ResultT value, IProgress prog)
        {
            return deserializer.TryLoad(new FileInfo(fileName), out value, prog);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text)
        {
            return deserializer.Parse(text, null);
        }

        public static ResultT Parse<ResultT>(this IDeserializer<ResultT> deserializer, string text, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.Deserialize(stream, stream.Length, prog);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value)
        {
            return deserializer.TryParse(text, out value, null);
        }

        public static bool TryParse<ResultT>(this IDeserializer<ResultT> deserializer, string text, out ResultT value, IProgress prog)
        {
            var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
            return deserializer.TryDeserialize(stream, out value, stream.Length, prog);
        }

        public static ResultT Decode<ResultT>(this Stream stream, IDeserializer<ResultT> deserializer, IProgress prog)
        {
            return deserializer.Deserialize(stream, prog);
        }

        public static ResultT Decode<ResultT>(this Stream stream, IDeserializer<ResultT> deserializer)
        {
            return deserializer.Deserialize(stream);
        }

        public static async Task<ResultT> Decode<ResultT>(this Task<Stream> stream, IDeserializer<ResultT> deserializer, IProgress prog)
        {
            return deserializer.Deserialize(await stream, prog);
        }

        public static async Task<ResultT> Decode<ResultT>(this Task<Stream> stream, IDeserializer<ResultT> deserializer)
        {
            return deserializer.Deserialize(await stream);
        }
    }
}