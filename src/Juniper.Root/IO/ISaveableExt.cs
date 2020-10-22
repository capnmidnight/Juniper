using System;
using System.IO;

namespace Juniper.IO
{

    public static class ISaveableExt
    {
        public static void Save<T>(this T item, Stream outputStream, ISerializer<T> serializer)
            where T : ISaveable<T>
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            serializer.Serialize(outputStream, item);
        }

        public static void Save<T>(this T item, FileInfo outputFile, ISerializer<T> serializer)
            where T : ISaveable<T>
        {
            if (outputFile is null)
            {
                throw new ArgumentNullException(nameof(outputFile));
            }

            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            using var outputStream = outputFile.Create();
            item.Save(outputStream, serializer);
        }

        public static void Save<T>(this T item, string fileName, ISerializer<T> serializer)
            where T : ISaveable<T>
        {
            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            item.Save(new FileInfo(fileName), serializer);
        }

        public static void Save<T>(this T item, FileInfo outputFile)
            where T : ISaveable<T>
        {
            if (outputFile is null)
            {
                throw new ArgumentNullException(nameof(outputFile));
            }

            if (MediaType.Application.Json.GuessMatches(outputFile))
            {
                var json = new JsonFactory<T>();
                using var stream = outputFile.Create();
                item.Save(stream, json);
            }
            else if (MediaType.Application.Octet_Stream.GuessMatches(outputFile))
            {
                var bin = new BinaryFactory<T>();
                using var stream = outputFile.Create();
                item.Save(stream, bin);
            }
        }

        public static void Save<T>(this T item, string fileName)
            where T : ISaveable<T>
        {
            if (fileName is null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            if (fileName.Length == 0)
            {
                throw new ArgumentException("path must not be empty string", nameof(fileName));
            }

            item.Save(new FileInfo(fileName));
        }
    }
}
