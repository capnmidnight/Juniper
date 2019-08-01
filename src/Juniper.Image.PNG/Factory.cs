using System;
using System.IO;

using Hjg.Pngcs;

using Juniper.Serialization;

namespace Juniper.Image.PNG
{
    public class Factory : IFactory<ImageData>
    {
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