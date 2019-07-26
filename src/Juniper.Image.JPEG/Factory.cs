using System;
using System.IO;
using BitMiracle.LibJpeg;

namespace Juniper.Image.JPEG
{
    public class Factory : IFactory
    {
        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public RawImage Decode(Stream imageStream, bool flipImage)
        {
            var source = RawImage.DetermineSource(imageStream);
            using (var jpeg = new JpegImage(imageStream))
            {
                var stride = jpeg.Width * jpeg.ComponentsPerSample;
                int numRows = jpeg.Height;
                var data = new byte[numRows * stride];
                for (var i = 0; i < jpeg.Height; ++i)
                {
                    var rowIndex = RawImage.GetRowIndex(numRows, i, flipImage);
                    var row = jpeg.GetRow(rowIndex);
                    Array.Copy(row.ToBytes(), 0, data, i * stride, stride);
                }

                return new RawImage(
                    source,
                    jpeg.Width,
                    jpeg.Height,
                    data);
            }
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public void Encode(RawImage image, Stream outputStream, bool flipImage)
        {
            var rows = new SampleRow[image.dimensions.height];
            var buf = new byte[image.stride];
            for (int i = 0; i < image.dimensions.height; ++i)
            {
                var rowIndex = RawImage.GetRowIndex(image.dimensions.height, i, flipImage);
                Array.Copy(image.data, rowIndex, buf, 0, buf.Length);
                rows[i] = new BitMiracle.LibJpeg.SampleRow(
                    buf,
                    image.dimensions.width,
                    RawImage.BitsPerComponent,
                    (byte)image.components);
            }

            using (var jpeg = new JpegImage(rows, Colorspace.RGB))
            {
                var compression = new CompressionParameters
                {
                    Quality = 10,
                    SimpleProgressive = false,
                    SmoothingFactor = 2
                };
                jpeg.WriteJpeg(outputStream, compression);
            }
        }
    }
}