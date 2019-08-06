namespace System.IO
{
    public static class StreamExt
    {
        public static void CopyTo(this Stream inStream, FileInfo outFile)
        {
            using (var outStream = outFile.OpenWrite())
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
    }
}