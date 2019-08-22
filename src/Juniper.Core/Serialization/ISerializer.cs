using System.IO;
using System.Text;

using Juniper.Progress;

namespace Juniper.Serialization
{
    public interface ISerializer
    {
        void Serialize<T>(Stream stream, T value, IProgress prog = null);
    }

    public interface ISerializer<T>
    {
        void Serialize(Stream stream, T value, IProgress prog = null);
    }

    public static class ISerializerExt
    {
        public static byte[] Serialize<T>(this ISerializer serializer, T value, IProgress progress = null)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<T>(this ISerializer serializer, FileInfo file, T value, IProgress progress = null)
        {
            using (var stream = file.OpenWrite())
            {
                serializer.Serialize(stream, value, progress);
            }
        }

        public static void Save<T>(this ISerializer serializer, string fileName, T value, IProgress progress = null)
        {
            serializer.Save(new FileInfo(fileName), value, progress);
        }

        public static string ToString<T>(this ISerializer serializer, T value, IProgress progress = null)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, progress));
        }

        public static ISerializer<T> Specialize<T>(this ISerializer serializer)
        {
            return new SpecializedSerializer<T>(serializer);
        }

        public static byte[] Serialize<T>(this ISerializer<T> serializer, T value, IProgress progress = null)
        {
            using (var mem = new MemoryStream())
            {
                serializer.Serialize(mem, value, progress);
                mem.Flush();

                return mem.ToArray();
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, FileInfo file, T value, IProgress progress = null)
        {
            using (var stream = file.OpenWrite())
            {
                serializer.Serialize(stream, value, progress);
            }
        }

        public static void Save<T>(this ISerializer<T> serializer, string fileName, T value, IProgress progress = null)
        {
            serializer.Save(new FileInfo(fileName), value, progress);
        }

        public static string ToString<T>(this ISerializer<T> serializer, T value, IProgress progress = null)
        {
            return Encoding.UTF8.GetString(serializer.Serialize(value, progress));
        }
    }
}