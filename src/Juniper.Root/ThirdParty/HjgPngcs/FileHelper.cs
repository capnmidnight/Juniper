using System.IO;

namespace Hjg.Pngcs
{
    /// <summary>
    /// A few utility static methods to read and write files
    /// </summary>
    ///
    public static class FileHelper
    {
        public static Stream OpenFileForReading(string file)
        {
            if (file is null || !File.Exists(file))
            {
                throw new PngjInputException("Cannot open file for reading (" + file + ")");
            }

            return new FileStream(file, FileMode.Open);
        }

        public static Stream OpenFileForWriting(string file, bool allowOverwrite)
        {
            if (File.Exists(file) && !allowOverwrite)
            {
                throw new PngjOutputException("File already exists (" + file + ") and overwrite=false");
            }

            return new FileStream(file, FileMode.Create);
        }

        /// <summary>
        /// Given a filename and a ImageInfo, produces a PngWriter object, ready for writing.</summary>
        /// <param name="fileName">Path of file</param>
        /// <param name="imgInfo">ImageInfo object</param>
        /// <param name="allowOverwrite">Flag: if false and file exists, a PngjOutputException is thrown</param>
        /// <returns>A PngWriter object, ready for writing</returns>
        public static PngWriter CreatePngWriter(string fileName, ImageInfo imgInfo, bool allowOverwrite)
        {
            return new PngWriter(OpenFileForWriting(fileName, allowOverwrite), imgInfo,
                    fileName);
        }

        /// <summary>
        /// Given a filename, produces a PngReader object, ready for reading.
        /// </summary>
        /// <param name="fileName">Path of file</param>
        /// <returns>PngReader, ready for reading</returns>
        public static PngReader CreatePngReader(string fileName)
        {
            return new PngReader(OpenFileForReading(fileName), fileName);
        }
    }
}