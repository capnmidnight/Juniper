using System.IO;
using System.Text;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ISerializer
    {
        void Serialize<InputT>(Stream stream, InputT value, IProgress prog);
    }

    public interface ISerializer<InputT>
    {
        void Serialize(Stream stream, InputT value, IProgress prog);
    }

    public static class ISerializerExt
    {
        public static void Serialize<InputT>(this ISerializer serializer, Stream stream, InputT value)
        {
            serializer.Serialize(stream, value, null);
        }

        public static byte[] Serialize<InputT>(this ISerializer serializer, InputT value)
        {
            return serializer.Serialize(value, null);
        }

        public static byte[] Serialize<InputT>(this ISerializer serializer, InputT value, IProgress progress)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<InputT>(this ISerializer serializer, FileInfo file, InputT value)
        {
            serializer.Save(file, value, null);
        }

        public static void Save<InputT>(this ISerializer serializer, FileInfo file, InputT value, IProgress progress)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, progress);
            }
        }

        public static void Save<InputT>(this ISerializer serializer, string fileName, InputT value)
        {
            serializer.Save(fileName, value, null);
        }

        public static void Save<InputT>(this ISerializer serializer, string fileName, InputT value, IProgress progress)
        {
            serializer.Save(new FileInfo(fileName), value, progress);
        }

        public static string ToString<InputT>(this ISerializer serializer, InputT value)
        {
            return serializer.ToString(value, null);
        }

        public static string ToString<InputT>(this ISerializer serializer, InputT value, IProgress progress)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, progress));
        }

        public static void Serialize<InputT>(this ISerializer<InputT> serializer, Stream stream, InputT value)
        {
            serializer.Serialize(stream, value, null);
        }

        public static byte[] Serialize<InputT>(this ISerializer<InputT> serializer, InputT value)
        {
            return serializer.Serialize(value, null);
        }

        public static byte[] Serialize<InputT>(this ISerializer<InputT> serializer, InputT value, IProgress progress)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<InputT>(this ISerializer<InputT> serializer, FileInfo file, InputT value)
        {
            serializer.Save(file, value, null);
        }

        public static void Save<InputT>(this ISerializer<InputT> serializer, FileInfo file, InputT value, IProgress progress)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, progress);
            }
        }

        public static void Save<InputT>(this ISerializer<InputT> serializer, string fileName, InputT value)
        {
            serializer.Save(fileName, value, null);
        }

        public static void Save<InputT>(this ISerializer<InputT> serializer, string fileName, InputT value, IProgress progress)
        {
            serializer.Save(new FileInfo(fileName), value, progress);
        }

        public static string ToString<InputT>(this ISerializer<InputT> serializer, InputT value)
        {
            return serializer.ToString(value, null);
        }

        public static string ToString<InputT>(this ISerializer<InputT> serializer, InputT value, IProgress progress)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, progress));
        }
    }
}