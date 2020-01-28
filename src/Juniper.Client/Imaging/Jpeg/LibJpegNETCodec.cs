using System;
using System.IO;

using BitMiracle.LibJpeg;

namespace Juniper.Imaging
{
    public class LibJpegNETCodec : IImageCodec<JpegImage>
    {
        private readonly CompressionParameters compressionParams;

        public LibJpegNETCodec(int quality = 100, int smoothingFactor = 1, bool progressive = false)
        {
            compressionParams = new CompressionParameters
            {
                Quality = quality,
                SimpleProgressive = progressive,
                SmoothingFactor = smoothingFactor
            };
        }

        public MediaType.Image ContentType =>
            MediaType.Image.Jpeg;

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="stream">Jpeg bytes.</param>
        public JpegImage Deserialize(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            return new JpegImage(stream);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="stream">Jpeg bytes.</param>
        public long Serialize(Stream stream, JpegImage value)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            value.WriteJpeg(stream, compressionParams);
            return stream.Length;
        }
    }
}