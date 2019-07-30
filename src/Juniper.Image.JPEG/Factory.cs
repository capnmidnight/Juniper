using System;
using System.IO;

using BitMiracle.LibJpeg;

using Juniper.Serialization;

namespace Juniper.Image.JPEG
{
    public class Factory : IFactory<RawImage>
    {
        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public RawImage Deserialize(Stream imageStream)
        {
            var source = RawImage.DetermineSource(imageStream);
            using (var jpeg = new JpegImage(imageStream))
            {
                var stride = jpeg.Width * jpeg.ComponentsPerSample;
                int numRows = jpeg.Height;
                var data = new byte[numRows * stride];
                for (var i = 0; i < jpeg.Height; ++i)
                {
                    var rowIndex = RawImage.GetRowIndex(numRows, i, true);
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
        public void Serialize(Stream outputStream, RawImage image)
        {
            var rows = new SampleRow[image.dimensions.height];
            var rowBuffer = new byte[image.stride];
            for (int i = 0; i < image.dimensions.height; ++i)
            {
                var rowIndex = RawImage.GetRowIndex(image.dimensions.height, i, true);
                var imageDataIndex = rowIndex * image.stride;
                Array.Copy(image.data, imageDataIndex, rowBuffer, 0, rowBuffer.Length);
                rows[i] = new SampleRow(
                    rowBuffer,
                    image.dimensions.width,
                    RawImage.BitsPerComponent,
                    (byte)image.components);
            }

            using (var jpeg = new JpegImage(rows, Colorspace.RGB))
            {
                var compression = new CompressionParameters
                {
                    Quality = 100,
                    SimpleProgressive = false,
                    SmoothingFactor = 1
                };
                jpeg.WriteJpeg(outputStream, compression);
            }
        }
    }
}