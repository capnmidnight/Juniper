using System.IO;
using System.Text;

using Juniper.Progress;

namespace Juniper.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T value);
    }

    public interface ISerializer<T>
    {
        void Serialize(Stream stream, T value);
    }

    public static class ISerializerExt
    {
        public static void Serialize<T>(this ISerializer serializer, Stream stream, T value, long length, IProgress progress)
        {
            serializer.Serialize(new ProgressStream(stream, length, progress), value);
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T value)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T value, long length, IProgress progress)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, length, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<T>(this ISerializer serializer, FileInfo file, T value)
        {
            using (var stream = file.OpenWrite())
            {
                serializer.Serialize(stream, value);
            }
        }

        public static void Save<T>(this ISerializer serializer, FileInfo file, T value, long length, IProgress progress)
        {
            using (var stream = file.OpenWrite())
            {
                serializer.Serialize(stream, value, length, progress);
            }
        }

        public static void Save<T>(this ISerializer serializer, string fileName, T value)
        {
            serializer.Save(new FileInfo(fileName), value);
        }

        public static void Save<T>(this ISerializer serializer, string fileName, T value, long length, IProgress progress)
        {
            serializer.Save(new FileInfo(fileName), value, length, progress);
        }

        public static string ToString<T>(this ISerializer serializer, T value)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value));
        }

        public static string ToString<T>(this ISerializer serializer, T value, long length, IProgress progress)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value));
        }

        public static ISerializer<T> Specialize<T>(this ISerializer serializer)
        {
            return new SpecializedSerializer<T>(serializer);
        }

        public static void Serialize<T>(this ISerializer<T> serializer, Stream stream, T value, long length, IProgress progress)
        {
            serializer.Serialize(new ProgressStream(stream, length, progress), value, length, progress);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value, long length, IProgress progress)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, length, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, FileInfo file, T value)
        {
            using (var stream = file.OpenWrite())
            {
                serializer.Serialize(stream, value);
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, FileInfo file, T value, long length, IProgress progress)
        {
            using (var stream = file.OpenWrite())
            {
                serializer.Serialize(stream, value, length, progress);
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, string fileName, T value)
        {
            serializer.Save(new FileInfo(fileName), value);
        }

        public static void Save<T>(this ISerializer<T> serializer, string fileName, T value, long length, IProgress progress)
        {
            serializer.Save(new FileInfo(fileName), value, length, progress);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value));
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value, long length, IProgress progress)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value));
        }
    }
}