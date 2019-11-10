using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.IO
{
    public interface ISaveable<T> : ISerializable
    { }

    public static class ISaveableExt
    {
        public static void Save<T>(this T obj, Stream outputStream, ISerializer<T> serializer)
            where T : ISaveable<T>
        {
            serializer.Serialize(outputStream, obj);
        }

        public static void Save<T>(this T obj, FileInfo outputFile, ISerializer<T> serializer)
            where T : ISaveable<T>
        {
            using (var outputStream = outputFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            {
                obj.Save(outputStream, serializer);
            }
        }

        public static void Save<T>(this T obj, string outputPath, ISerializer<T> serializer)
            where T : ISaveable<T>
        {
            obj.Save(new FileInfo(outputPath), serializer);
        }

        public static void Save<T>(this T obj, Stream outputStream)
            where T : ISaveable<T>
        {
            var json = new JsonFactory<T>();
            obj.Save(outputStream, json);
        }

        public static void Save<T>(this T obj, FileInfo outputFile)
            where T : ISaveable<T>
        {
            using(var outputStream = outputFile.Open(FileMode.Create, FileAccess.Write, FileShare.None))
            {
                obj.Save(outputStream);
            }
        }

        public static void Save<T>(this T obj, string outputPath)
            where T : ISaveable<T>
        {
            obj.Save(new FileInfo(outputPath));
        }
    }
}
