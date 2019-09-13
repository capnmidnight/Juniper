using System.IO;
using System.Text;

using Juniper.Progress;

namespace Juniper.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T value, IProgress prog);
    }

    public interface ISerializer<T>
    {
        void Serialize(Stream stream, T value, IProgress prog);
    }

    public static class ISerializerExt
    {
        public static void Serialize<T>(this ISerializer serializer, Stream stream, T value)
        {
            serializer.Serialize(stream, value, null);
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T value)
        {
            return serializer.Serialize(value, null);
        }

        public static byte[] Serialize<T>(this ISerializer serializer, T value, IProgress progress)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<T>(this ISerializer serializer, FileInfo file, T value)
        {
            serializer.Save(file, value);
        }

        public static void Save<T>(this ISerializer serializer, FileInfo file, T value, IProgress progress)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, progress);
            }
        }

        public static void Save<T>(this ISerializer serializer, string fileName, T value)
        {
            serializer.Save(fileName, value);
        }

        public static void Save<T>(this ISerializer serializer, string fileName, T value, IProgress progress)
        {
            serializer.Save(new FileInfo(fileName), value, progress);
        }

        public static string ToString<T>(this ISerializer serializer, T value)
        {
            return serializer.ToString(value);
        }

        public static string ToString<T>(this ISerializer serializer, T value, IProgress progress)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, progress));
        }

        public static ISerializer<T> Specialize<T>(this ISerializer serializer)
        {
            return new SpecializedSerializer<T>(serializer);
        }
        public static void Serialize<T>(this ISerializer<T> serializer, Stream stream, T value)
        {
            serializer.Serialize(stream, value, null);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value)
        {
            return serializer.Serialize(value, null);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value, IProgress progress)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, FileInfo file, T value)
        {
            serializer.Save(file, value, null);
        }

        public static void Save<T>(this ISerializer<T> serializer, FileInfo file, T value, IProgress progress)
        {
            using (var stream = file.Create())
            {
                serializer.Serialize(stream, value, progress);
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, string fileName, T value)
        {
            serializer.Save(fileName, value, null);
        }

        public static void Save<T>(this ISerializer<T> serializer, string fileName, T value, IProgress progress)
        {
            serializer.Save(new FileInfo(fileName), value, progress);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value)
        {
            return serializer.ToString(value, null);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value, IProgress progress)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, progress));
        }
    }
}