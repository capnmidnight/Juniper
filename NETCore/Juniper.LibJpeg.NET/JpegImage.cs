using BitMiracle.LibJpeg.Classic;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace BitMiracle.LibJpeg
{
    /// <summary>
    /// Main class for work with JPEG images.
    /// </summary>
    public sealed class JpegImage : IDisposable
    {
        private bool m_alreadyDisposed;

        /// <summary>
        /// Description of image pixels (samples)
        /// </summary>
        private List<SampleRow> m_rows = new List<SampleRow>();

        // Fields below (m_compressedData, m_decompressedData, m_bitmap) are not initialized in constructors necessarily.
        // Instead direct access to these field you should use corresponding properties (compressedData, decompressedData, bitmap)
        // Such agreement allows to load required data (e.g. compress image) only by request.

        /// <summary>
        /// Bytes of jpeg image. Refreshed when m_compressionParameters changed.
        /// </summary>
        private MemoryStream m_compressedData;

        /// <summary>
        /// Current compression parameters corresponding with compressed data.
        /// </summary>
        private CompressionParameters m_compressionParameters;

        /// <summary>
        /// Bytes of decompressed image (bitmap)
        /// </summary>
        private MemoryStream m_decompressedData;

        /// <summary>
        /// Creates <see cref="JpegImage"/> from stream with an arbitrary image data
        /// </summary>
        /// <param name="imageData">Stream containing bytes of image in
        /// arbitrary format (BMP, Jpeg, GIF, PNG, TIFF, e.t.c)</param>
        public JpegImage(Stream imageData)
        {
            if (imageData is null)
            {
                throw new ArgumentNullException(nameof(imageData));
            }

            m_compressedData = new MemoryStream();
            imageData.CopyTo(m_compressedData);

            if (CompressedData.Length <= 2)
            {
                throw new ArgumentException("There must be at least two bytes in the image stream", nameof(imageData));
            }

            CompressedData.Seek(0, SeekOrigin.Begin);
            var first = CompressedData.ReadByte();
            var second = CompressedData.ReadByte();

            if (!(first == 0xFF)
                && (second == (int)JpegMarker.SOI))
            {
                throw new ArgumentException("This is not a JPEG stream", nameof(imageData));
            }

            Decompress();
        }

        /// <summary>
        /// Creates <see cref="JpegImage"/> from pixels
        /// </summary>
        /// <param name="sampleData">Description of pixels.</param>
        /// <param name="colorspace">Colorspace of image.</param>
        /// <seealso cref="SampleRow"/>
        public JpegImage(SampleRow[] sampleData, Colorspace colorspace)
        {
            if (sampleData is null)
            {
                throw new ArgumentNullException(nameof(sampleData));
            }

            if (sampleData.Length == 0)
            {
                throw new ArgumentException("sampleData must be no empty");
            }

            if (colorspace == Colorspace.Unknown)
            {
                throw new ArgumentException("Unknown colorspace");
            }

            m_rows = new List<SampleRow>(sampleData);

            var firstRow = m_rows[0];
            Width = firstRow.Length;
            Height = m_rows.Count;

            var firstSample = firstRow[0];
            BitsPerComponent = firstSample.BitsPerComponent;
            ComponentsPerSample = firstSample.ComponentCount;
            Colorspace = colorspace;
        }

        /// <summary>
        /// Frees and releases all resources allocated by this <see cref="JpegImage"/>
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (!m_alreadyDisposed)
            {
                if (disposing)
                {
                    // dispose managed resources
                    m_compressedData?.Dispose();
                    m_decompressedData?.Dispose();
                }

                // free native resources
                m_compressionParameters = null;
                m_compressedData = null;
                m_decompressedData = null;
                m_rows = null;
                m_alreadyDisposed = true;
            }
        }

        /// <summary>
        /// Gets the width of image in <see cref="Sample">samples</see>.
        /// </summary>
        /// <value>The width of image.</value>
        public int Width { get; internal set; }

        /// <summary>
        /// Gets the height of image in <see cref="Sample">samples</see>.
        /// </summary>
        /// <value>The height of image.</value>
        public int Height { get; internal set; }

        /// <summary>
        /// Gets the number of color components per <see cref="Sample">sample</see>.
        /// </summary>
        /// <value>The number of color components per sample.</value>
        public byte ComponentsPerSample { get; internal set; }

        /// <summary>
        /// Gets the number of bits per color component of <see cref="Sample">sample</see>.
        /// </summary>
        /// <value>The number of bits per color component.</value>
        public byte BitsPerComponent { get; internal set; }

        /// <summary>
        /// Gets the colorspace of image.
        /// </summary>
        /// <value>The colorspace of image.</value>
        public Colorspace Colorspace { get; internal set; }

        /// <summary>
        /// Retrieves the required row of image.
        /// </summary>
        /// <param name="rowNumber">The number of row.</param>
        /// <returns>Image row of samples.</returns>
        public SampleRow GetRow(int rowNumber)
        {
            return m_rows[rowNumber];
        }

        /// <summary>
        /// Writes compressed JPEG image to stream.
        /// </summary>
        /// <param name="output">Output stream.</param>
        public void WriteJpeg(Stream output)
        {
            WriteJpeg(output, new CompressionParameters());
        }

        /// <summary>
        /// Compresses image to JPEG with given parameters and writes it to stream.
        /// </summary>
        /// <param name="output">Output stream.</param>
        /// <param name="parameters">The parameters of compression.</param>
        public void WriteJpeg(Stream output, CompressionParameters parameters)
        {
            Compress(parameters);
            CompressedData.WriteTo(output);
        }

        /// <summary>
        /// Writes decompressed image data as bitmap to stream.
        /// </summary>
        /// <param name="output">Output stream.</param>
        public void WriteBitmap(Stream output)
        {
            DecompressedData.WriteTo(output);
        }

        private MemoryStream CompressedData
        {
            get
            {
                if (m_compressedData is null)
                {
                    Compress(new CompressionParameters());
                }

                Debug.Assert(m_compressedData is object);
                Debug.Assert(m_compressedData.Length != 0);

                return m_compressedData;
            }
        }

        private MemoryStream DecompressedData
        {
            get
            {
                if (m_decompressedData is null)
                {
                    FillDecompressedData();
                }

                Debug.Assert(m_decompressedData is object);

                return m_decompressedData;
            }
        }

        /// <summary>
        /// Needs for DecompressorToJpegImage class
        /// </summary>
        internal void AddSampleRow(SampleRow row)
        {
            if (row is null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            m_rows.Add(row);
        }

        private void Compress(CompressionParameters parameters)
        {
            Debug.Assert(m_rows is object);
            Debug.Assert(m_rows.Count != 0);

            var source = new RawImage(m_rows, Colorspace);
            Compress(source, parameters);
        }

        private void Compress(IRawImage source, CompressionParameters parameters)
        {
            Debug.Assert(source is object);

            if (!NeedCompressWith(parameters))
            {
                return;
            }

            m_compressedData = new MemoryStream();
            m_compressionParameters = new CompressionParameters(parameters);

            var jpeg = new Jpeg
            {
                CompressionParameters = m_compressionParameters
            };
            jpeg.Compress(source, m_compressedData);
        }

        private bool NeedCompressWith(CompressionParameters parameters)
        {
            return m_compressedData is null
                || m_compressionParameters?.Equals(parameters) != true;
        }

        private void Decompress()
        {
            var jpeg = new Jpeg();
            jpeg.Decompress(CompressedData, new DecompressorToJpegImage(this));
        }

        private void FillDecompressedData()
        {
            Debug.Assert(m_decompressedData is null);

            m_decompressedData = new MemoryStream();
            var dest = new BitmapDestination(m_decompressedData);

            var jpeg = new Jpeg();
            jpeg.Decompress(CompressedData, dest);
        }
    }
}
