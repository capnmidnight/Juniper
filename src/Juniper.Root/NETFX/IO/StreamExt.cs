using System.Threading.Tasks;

namespace System.IO
{
    public static class StreamExt
    {
        public static void CopyTo(this Stream inStream, FileInfo outFile)
        {
            if (inStream is null)
            {
                throw new ArgumentNullException(nameof(inStream));
            }

            if (outFile is null)
            {
                throw new ArgumentNullException(nameof(outFile));
            }

            using var outStream = outFile.Create();
            inStream.CopyTo(outStream);
        }

        public static async Task CopyToAsync(this Stream inStream, FileInfo outFile)
        {
            if (inStream is null)
            {
                throw new ArgumentNullException(nameof(inStream));
            }

            if (outFile is null)
            {
                throw new ArgumentNullException(nameof(outFile));
            }

            using var outStream = outFile.Create();
            await inStream
                .CopyToAsync(outStream)
                .ConfigureAwait(false);
        }

        public static void CopyTo(this Stream inStream, string outFileName)
        {
            if (inStream is null)
            {
                throw new ArgumentNullException(nameof(inStream));
            }

            inStream.CopyTo(new FileInfo(outFileName.ValidateFileName()));
        }

        public static Task CopyToAsync(this Stream inStream, string outFileName)
        {
            if (inStream is null)
            {
                throw new ArgumentNullException(nameof(inStream));
            }

            return inStream.CopyToAsync(new FileInfo(outFileName.ValidateFileName()));
        }

        public static void CopyTo(this FileInfo inFile, Stream outStream)
        {
            if (inFile is null)
            {
                throw new ArgumentNullException(nameof(inFile));
            }

            using var inStream = inFile.OpenRead();
            inStream.CopyTo(outStream);
        }

        public static async Task CopyToAsync(this FileInfo inFile, Stream outStream)
        {
            if (inFile is null)
            {
                throw new ArgumentNullException(nameof(inFile));
            }

            using var inStream = inFile.OpenRead();
            await inStream
                .CopyToAsync(outStream)
                .ConfigureAwait(false);
        }

        public static void CopyTo(this FileInfo inFile, FileInfo outFile)
        {
            if (inFile is null)
            {
                throw new ArgumentNullException(nameof(inFile));
            }

            if (outFile is null)
            {
                throw new ArgumentNullException(nameof(outFile));
            }

            _ = inFile.CopyTo(outFile.FullName, true);
        }
    }
}