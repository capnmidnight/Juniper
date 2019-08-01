using System;
using System.IO;

using Hjg.Pngcs;

using Juniper.Serialization;

namespace Juniper.Image.PNG
{
    public class Factory : IFactory<ImageData>
    {
        public static Size ReadDimensions(byte[] data)
        {
            int i = 8; // skip the PNG signature

            while (i < data.Length)
            {
                int len = 0;
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];
                len = len << 8 | data[i++];

                var chunk = System.Text.Encoding.UTF8.GetString(data, i, 4);
                i += 4;

                if (chunk == "IHDR")
                {
                    int width = 0, height = 0;
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];
                    width = width << 8 | data[i++];

                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];
                    height = height << 8 | data[i++];

                    return new Size(width, height);
                }

                i += len;

                i += 4;
            }

            return default;
        }

        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public ImageData Deserialize(Stream imageStream)
        {
            var source = ImageData.DetermineSource(imageStream);
            var png = new PngReader(imageStream);
            png.SetUnpackedMode(true);
            var rows = png.ReadRowsByte();
            int numRows = rows.Nrows;
            var data = new byte[numRows * rows.elementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                var rowIndex = ImageData.GetRowIndex(numRows, i, true);
                var row = rows.ScanlinesB[rowIndex];
                Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
            }

            return new ImageData(
                source,
                ImageFormat.PNG,
                rows.elementsPerRow / rows.channels,
                numRows,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public void Serialize(Stream outputStream, ImageData image)
        {
            var info = new ImageInfo(
                image.dimensions.width,
                image.dimensions.height,
                ImageData.BitsPerComponent,
                false);

            var png = new PngWriter(outputStream, info)
            {
                IdatMaxSize = 0x1000,
                CompLevel = 8
            };

            png.SetFilterType(FilterType.FILTER_PAETH);

            var metadata = png.GetMetadata();
            metadata.SetDpi(100);

            var line = new ImageLine(info, ImageLine.ESampleType.BYTE);
            for (var i = 0; i < image.dimensions.height; ++i)
            {
                var row = ImageData.GetRowIndex(image.dimensions.height, i, true);
                Array.Copy(image.data, row * image.stride, line.ScanlineB, 0, image.stride);
                png.WriteRow(line, i);
            }

            png.End();
        }
    }
}