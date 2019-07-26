using System;
using System.IO;
using Hjg.Pngcs;

namespace Juniper.Image.PNG
{
    public class Factory : IFactory
    {
        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public RawImage Decode(Stream imageStream, bool flipImage)
        {
            var source = RawImage.DetermineSource(imageStream);
            var png = new PngReader(imageStream);
            png.SetUnpackedMode(true);
            var rows = png.ReadRowsByte();
            int numRows = rows.Nrows;
            var data = new byte[numRows * rows.elementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                var rowIndex = RawImage.GetRowIndex(numRows, i, flipImage);
                var row = rows.ScanlinesB[rowIndex];
                Array.Copy(row, 0, data, i * rows.elementsPerRow, row.Length);
            }

            return new RawImage(
                source,
                rows.elementsPerRow / rows.channels,
                numRows,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public void Encode(RawImage image, Stream outputStream, bool flipImage)
        {
            var info = new ImageInfo(
                image.dimensions.width,
                image.dimensions.height,
                RawImage.BitsPerComponent,
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
                var row = RawImage.GetRowIndex(image.dimensions.height, i, flipImage);
                Array.Copy(image.data, row * image.stride, line.ScanlineB, 0, image.stride);
                png.WriteRow(line, i);
            }

            png.End();
        }
    }
}