using System;

using Hjg.Pngcs;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public class HjgPngcsImageDataTranscoder : IImageTranscoder<ImageLines, ImageData>
    {
        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public ImageData Translate(ImageLines value, IProgress prog)
        {
            var numRows = value.Nrows;
            var data = new byte[numRows * value.elementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                prog.Report(i, numRows);
                var row = value.ScanlinesB[i];
                Array.Copy(row, 0, data, i * value.elementsPerRow, row.Length);
                prog.Report(i + 1, numRows);
            }

            return new ImageData(
                value.ImgInfo.BytesPerRow / value.ImgInfo.BytesPixel,
                value.Nrows,
                value.channels,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public ImageLines Translate(ImageData image, IProgress prog)
        {
            var imageInfo = new Hjg.Pngcs.ImageInfo(
                image.info.dimensions.width,
                image.info.dimensions.height,
                image.info.bitsPerSample / image.info.components,
                image.info.components == 4);

            var imageLines = new ImageLines(
                imageInfo,
                ImageLine.ESampleType.BYTE,
                true,
                0,
                image.info.dimensions.height,
                image.info.stride);

            for (var y = 0; y < image.info.dimensions.height; ++y)
            {
                prog.Report(y, image.info.dimensions.height);
                var dataIndex = y * image.info.stride;
                var line = imageLines.ScanlinesB[y];
                Array.Copy(image.data, dataIndex, line, 0, image.info.stride);
                prog.Report(y + 1, image.info.dimensions.height);
            }

            return imageLines;
        }
    }
}
