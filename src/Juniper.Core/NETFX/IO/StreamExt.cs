using Juniper.IO;
using Juniper.Progress;

namespace System.IO
{
    public static class StreamExt
    {
        public static void CopyTo(this Stream inStream, FileInfo outFile)
        {
            using (var outStream = outFile.Create())
            {
                inStream.CopyTo(outStream);
            }
        }

        public static void CopyTo(this Stream inStream, string outFileName)
        {
            inStream.CopyTo(new FileInfo(outFileName));
        }

        public static void CopyTo(this FileInfo inFile, Stream outStream)
        {
            using (var inStream = inFile.OpenRead())
            {
                inStream.CopyTo(outStream);
            }
        }

        public static void CopyTo(this FileInfo inFile, FileInfo outFile)
        {
            inFile.CopyTo(outFile.FullName, true);
        }

        public static T Decode<T>(this Stream stream, IDeserializer<T> deserializer, IProgress prog)
        {
            if (stream == null)
            {
                return default;
            }
            else
            {
                using (stream)
                {
                    return deserializer.Deserialize(stream, prog);
                }
            }
        }

        public static T Decode<T>(this Stream stream, IDeserializer<T> deserializer)
        {
            if (stream == null)
            {
                return default;
            }
            else
            {
                using (stream)
                {
                    return deserializer.Deserialize(stream);
                }
            }
        }
    }
}