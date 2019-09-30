using System;
using System.IO;

using BitMiracle.LibJpeg;
using Juniper.HTTP;
using Juniper.Progress;
using Juniper.Streams;

namespace Juniper.Imaging.LibJpegNET
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

        public ImageInfo GetImageInfo(byte[] data)
        {
            return ImageInfo.ReadJPEG(data);
        }

        public MediaType.Image ImageType
        {
            get
            {
                return MediaType.Image.Jpeg;
            }
        }

        public MediaType ContentType
        {
            get
            {
                return ImageType;
            }
        }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public JpegImage Deserialize(Stream imageStream, IProgress prog)
        {
            prog.Report(0);
            using (var seekable = new ErsatzSeekableStream(imageStream))
            {
                var img = new JpegImage(seekable);
                prog.Report(1);
                return img;
            }
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public void Serialize(Stream outputStream, JpegImage image, IProgress prog)
        {
            prog.Report(0);
            image.WriteJpeg(outputStream, compressionParams);
            prog.Report(1);
        }

        public int GetWidth(JpegImage img)
        {
            return img.Width;
        }

        public int GetHeight(JpegImage img)
        {
            return img.Height;
        }

        public int GetComponents(JpegImage img)
        {
            return img.ComponentsPerSample;
        }

        public JpegImage Concatenate(JpegImage[,] images, IProgress prog)
        {
            this.ValidateImages(images, prog,
                out var rows, out var columns, out var components,
                out var tileWidth,
                out var tileHeight);

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
                        if (tile != null)
                        {
                            images[tileY, tileX] = null;
                            var tileRowBuffer = tile.GetRow(y).ToBytes();
                            Array.Copy(tileRowBuffer, 0, rowBuffer, tileX * tileWidth * components, tileRowBuffer.Length);
                            tile.Dispose();
                            GC.Collect();
                        }
                    }
                    combined[tileY * tileHeight + y] = new SampleRow(rowBuffer, bufferWidth, 8, (byte)components);
                }
            }

            return new JpegImage(combined, Colorspace.RGB);
        }
    }
}