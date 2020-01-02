using System.IO;

using BitMiracle.LibJpeg;

using Juniper.IO;
using Juniper.Progress;

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

        public MediaType.Image ContentType
        {
            get { return MediaType.Image.Jpeg; }
        }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="stream">Jpeg bytes.</param>
        public JpegImage Deserialize(Stream stream, IProgress prog)
        {
            prog.Report(0);
            JpegImage image = null;
            if (stream is object)
            {
                using var seekable = new ErsatzSeekableStream(stream);
                image = new JpegImage(seekable);
            }

            prog.Report(1);
            return image;
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="stream">Jpeg bytes.</param>
        public void Serialize(Stream stream, JpegImage value, IProgress prog = null)
        {
            prog.Report(0);
            value.WriteJpeg(stream, compressionParams);
            prog.Report(1);
        }
    }
}