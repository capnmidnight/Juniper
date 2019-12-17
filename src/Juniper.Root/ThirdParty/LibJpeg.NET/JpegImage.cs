using System;
using System.Collections.Generic;
using System.Diagnostics;

#if !NETSTANDARD
// namespace System.Drawing.* is not available in .NET Standard
using System.Drawing;
using System.Drawing.Imaging;
#endif

using System.IO;

using BitMiracle.LibJpeg.Classic;

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

#if !NETSTANDARD
        /// <summary>
        /// .NET bitmap associated with this image
        /// </summary>
        private Bitmap m_bitmap;
#endif

#if !NETSTANDARD
        /// <summary>
        /// Creates <see cref="JpegImage"/> from <see cref="System.Drawing.Bitmap">.NET bitmap</see>
        /// </summary>
        /// <param name="bitmap">Source .NET bitmap.</param>
        public JpegImage(System.Drawing.Bitmap bitmap)
        {
            createFromBitmap(bitmap);
        }

        /// <summary>
        /// Creates <see cref="JpegImage"/> from file with an arbitrary image
        /// </summary>
        /// <param name="fileName">Path to file with image in 
        /// arbitrary format (BMP, Jpeg, GIF, PNG, TIFF, e.t.c)</param>
        public JpegImage(string fileName)
        {
            if (fileName == null)
            {
                throw new ArgumentNullException(nameof(fileName));
            }

            using (var input = new FileStream(fileName, FileMode.Open))
            {
                createFromStream(input);
            }
        }
#endif

        /// <summary>
        /// Creates <see cref="JpegImage"/> from stream with an arbitrary image data
        /// </summary>
        /// <param name="imageData">Stream containing bytes of image in 
        /// arbitrary format (BMP, Jpeg, GIF, PNG, TIFF, e.t.c)</param>
        public JpegImage(Stream imageData)
        {
            createFromStream(imageData);
        }

        /// <summary>
        /// Creates <see cref="JpegImage"/> from pixels
        /// </summary>
        /// <param name="sampleData">Description of pixels.</param>
        /// <param name="colorspace">Colorspace of image.</param>
        /// <seealso cref="SampleRow"/>
        public JpegImage(SampleRow[] sampleData, Colorspace colorspace)
        {
            if (sampleData == null)
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

#if !NETSTANDARD
        /// <summary>
        /// Creates <see cref="JpegImage"/> from <see cref="System.Drawing.Bitmap">.NET bitmap</see>
        /// </summary>
        /// <param name="bitmap">Source .NET bitmap.</param>
        /// <returns>Created instance of <see cref="JpegImage"/> class.</returns>
        /// <remarks>Same as corresponding <see cref="M:BitMiracle.LibJpeg.JpegImage.#ctor(System.Drawing.Bitmap)">constructor</see>.</remarks>
        public static JpegImage FromBitmap(Bitmap bitmap)
        {
            return new JpegImage(bitmap);
        }
#endif

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

#if !NETSTANDARD
                    m_bitmap?.Dispose();
#endif
                }

                // free native resources
                m_compressionParameters = null;
                m_compressedData = null;
                m_decompressedData = null;
#if !NETSTANDARD                
                m_bitmap = null;
#endif
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
            compress(parameters);
            compressedData.WriteTo(output);
        }

        /// <summary>
        /// Writes decompressed image data as bitmap to stream.
        /// </summary>
        /// <param name="output">Output stream.</param>
        public void WriteBitmap(Stream output)
        {
            decompressedData.WriteTo(output);
        }

#if !NETSTANDARD
        /// <summary>
        /// Retrieves image as .NET Bitmap.
        /// </summary>
        /// <returns>.NET Bitmap</returns>
        public Bitmap ToBitmap()
        {
            return bitmap.Clone() as Bitmap;
        }
#endif

        private MemoryStream compressedData
        {
            get
            {
                if (m_compressedData == null)
                {
                    compress(new CompressionParameters());
                }

                Debug.Assert(m_compressedData != null);
                Debug.Assert(m_compressedData.Length != 0);

                return m_compressedData;
            }
        }

        private MemoryStream decompressedData
        {
            get
            {
                if (m_decompressedData == null)
                {
                    fillDecompressedData();
                }

                Debug.Assert(m_decompressedData != null);

                return m_decompressedData;
            }
        }

#if !NETSTANDARD
        private Bitmap bitmap
        {
            get
            {
                if (m_bitmap == null)
                {
                    var position = compressedData.Position;
                    m_bitmap = new Bitmap(compressedData);
                    compressedData.Seek(position, SeekOrigin.Begin);
                }

                return m_bitmap;
            }
        }
#endif

        /// <summary>
        /// Needs for DecompressorToJpegImage class
        /// </summary>
        internal void addSampleRow(SampleRow row)
        {
            if (row == null)
            {
                throw new ArgumentNullException(nameof(row));
            }

            m_rows.Add(row);
        }

        /// <summary>
        /// Checks if imageData contains jpeg image
        /// </summary>
        private static bool isCompressed(Stream imageData)
        {
            if (imageData == null)
            {
                return false;
            }

            if (imageData.Length <= 2)
            {
                return false;
            }

            imageData.Seek(0, SeekOrigin.Begin);
            var first = imageData.ReadByte();
            var second = imageData.ReadByte();
            return first == 0xFF && second == (int)JpegMarker.SOI;
        }

        private void createFromStream(Stream imageData)
        {
            if (imageData == null)
            {
                throw new ArgumentNullException(nameof(imageData));
            }

            if (isCompressed(imageData))
            {
                m_compressedData = Utils.CopyStream(imageData);
                decompress();
            }
            else
            {
#if !NETSTANDARD
                createFromBitmap(new Bitmap(imageData));
#else
                throw new NotImplementedException("JpegImage.createFromStream(Stream)");
#endif
            }
        }

#if !NETSTANDARD
        private void createFromBitmap(System.Drawing.Bitmap bitmap)
        {
            initializeFromBitmap(bitmap);
            compress(new CompressionParameters());
        }

        private void initializeFromBitmap(Bitmap bitmap)
        {
            m_bitmap = bitmap ?? throw new ArgumentNullException(nameof(bitmap));
            Width = m_bitmap.Width;
            Height = m_bitmap.Height;
            processPixelFormat(bitmap.PixelFormat);
            fillSamplesFromBitmap();
        }
#endif

        private void compress(CompressionParameters parameters)
        {
            Debug.Assert(m_rows != null);
            Debug.Assert(m_rows.Count != 0);

            var source = new RawImage(m_rows, Colorspace);
            compress(source, parameters);
        }

        private void compress(IRawImage source, CompressionParameters parameters)
        {
            Debug.Assert(source != null);

            if (!needCompressWith(parameters))
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

        private bool needCompressWith(CompressionParameters parameters)
        {
            return m_compressedData == null ||
                   m_compressionParameters?.Equals(parameters) != true;
        }

        private void decompress()
        {
            var jpeg = new Jpeg();
            jpeg.Decompress(compressedData, new DecompressorToJpegImage(this));
        }

        private void fillDecompressedData()
        {
            Debug.Assert(m_decompressedData == null);

            m_decompressedData = new MemoryStream();
            var dest = new BitmapDestination(m_decompressedData);

            var jpeg = new Jpeg();
            jpeg.Decompress(compressedData, dest);
        }

#if !NETSTANDARD
        private void processPixelFormat(PixelFormat pixelFormat)
        {
            //See GdiPlusPixelFormats.h for details

            if (pixelFormat == PixelFormat.Format16bppGrayScale)
            {
                BitsPerComponent = 16;
                ComponentsPerSample = 1;
                Colorspace = Colorspace.Grayscale;
                return;
            }

            var formatIndexByte = (byte)((int)pixelFormat & 0x000000FF);
            var pixelSizeByte = (byte)((int)pixelFormat & 0x0000FF00);

            if (pixelSizeByte == 32 && formatIndexByte == 15) //PixelFormat32bppCMYK (15 | (32 << 8))
            {
                BitsPerComponent = 8;
                ComponentsPerSample = 4;
                Colorspace = Colorspace.CMYK;
                return;
            }

            BitsPerComponent = 8;
            ComponentsPerSample = 3;
            Colorspace = Colorspace.RGB;

            if (pixelSizeByte == 16)
            {
                BitsPerComponent = 6;
            }
            else if (pixelSizeByte == 24 || pixelSizeByte == 32)
            {
                BitsPerComponent = 8;
            }
            else if (pixelSizeByte == 48 || pixelSizeByte == 64)
            {
                BitsPerComponent = 16;
            }
        }

        private void fillSamplesFromBitmap()
        {
            Debug.Assert(m_bitmap != null);

            for (var y = 0; y < Height; ++y)
            {
                var samples = new short[Width * 3];
                for (var x = 0; x < Width; ++x)
                {
                    var color = m_bitmap.GetPixel(x, y);
                    samples[x * 3] = color.R;
                    samples[x * 3 + 1] = color.G;
                    samples[x * 3 + 2] = color.B;
                }
                m_rows.Add(new SampleRow(samples, BitsPerComponent, ComponentsPerSample));
            }
        }
#endif
    }
}
