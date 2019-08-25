using System;
using System.IO;
using System.Linq;

using BitMiracle.LibJpeg;

using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.Imaging.LibJpegNET
{
    public class LibJpegNETCodec : IImageDecoder<JpegImage>
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

        public ImageInfo GetImageInfo(byte[] data)
        {
            return ImageInfo.ReadJPEG(data);
        }

        public HTTP.MediaType.Image Format { get { return HTTP.MediaType.Image.Jpeg; } }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public JpegImage Deserialize(Stream imageStream)
        {
            using (var seekable = new ErsatzSeekableStream(imageStream))
            {
                return new JpegImage(seekable);
            }
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public void Serialize(Stream outputStream, JpegImage image, IProgress prog = null)
        {
            prog?.Report(0);
            image.WriteJpeg(outputStream, compressionParams);
            prog?.Report(1);
        }

        public int GetWidth(JpegImage img)
        {
            return img.Width;
        }

        public int GetHeight(JpegImage img)
        {
            return img.Height;
        }

        public JpegImage Concatenate(JpegImage[,] images, IProgress prog = null)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns,
                out var tileWidth,
                out var tileHeight);

            var firstImage = images.Where(img => img != null).First();

            var bufferWidth = columns * tileWidth;
            var bufferHeight = rows * tileHeight;
            var combined = new SampleRow[bufferHeight];
            for (var tileY = 0; tileY < rows; ++tileY)
            {
                for (var y = 0; y < tileHeight; ++y)
                {
                    var rowBuffer = new byte[bufferWidth];
                    for (var tileX = 0; tileX < columns; ++tileX)
                    {
                        var tile = images[tileY, tileX];
                        var tileRowBuffer = tile.GetRow(y).ToBytes();
                        Array.Copy(tileRowBuffer, 0, rowBuffer, tileX * tileWidth * firstImage.ComponentsPerSample, tileRowBuffer.Length);
                    }
                    combined[tileY * tileHeight + y] = new SampleRow(rowBuffer, bufferWidth, firstImage.BitsPerComponent, firstImage.ComponentsPerSample);
                }
            }

            return new JpegImage(combined, firstImage.Colorspace);
        }
    }
}