using System.IO;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Common interface for processing of decompression.
    /// </summary>
    internal interface IDecompressDestination
    {
        /// <summary>
        /// Strean with decompressed data
        /// </summary>
        Stream Output { get; }

        /// <summary>
        /// Implementor of this interface should process image properties received from decompressor.
        /// </summary>
        /// <param name="parameters">Image properties</param>
        void SetImageAttributes(LoadedImageAttributes parameters);

        /// <summary>
        /// Called before decompression
        /// </summary>
        void BeginWrite();

        /// <summary>
        /// It called during decompression - pass row of pixels from JPEG
        /// </summary>
        /// <param name="row"></param>
        void ProcessPixelsRow(byte[] row);

        /// <summary>
        /// Called after decompression
        /// </summary>
        void EndWrite();
    }
}
