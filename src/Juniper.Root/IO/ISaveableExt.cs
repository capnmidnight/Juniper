namespace Juniper.IO
{

    public static class ISaveableExt
    {
        public static void Save<T, M>(this T item, Stream outputStream, ISerializer<T, M> serializer)
            where T : ISaveable<T>
            where M : MediaType
        {
            if (outputStream is null)
            {
                throw new ArgumentNullException(nameof(outputStream));
            }

            if (serializer is null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            _ = serializer.Serialize(outputStream, item);
        }

        public static void Save<T, M>(this T item, FileInfo outputFile, ISerializer<T, M> serializer)
            where T : ISaveable<T>
            where M : MediaType
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

        public static void Save<T, M>(this T item, string fileName, ISerializer<T, M> serializer)
            where T : ISaveable<T>
            where M : MediaType
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

            if (MediaType.Application_Json.GuessMatches(outputFile))
            {
                var json = new JsonFactory<T>();
                using var stream = outputFile.Create();
                item.Save(stream, json);
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
