using System;

using Hjg.Pngcs;
using Juniper.IO;

namespace Juniper.Imaging
{
    public class HjgPngcsImageDataTranscoder : IImageTranscoder<ImageLines, ImageData>
    {
        /// <summary>
        /// Decodes a raw file buffer of PNG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Png bytes.</param>
        public ImageData Translate(ImageLines value, IProgress prog = null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var numRows = value.Nrows;
            var data = new byte[numRows * value.ElementsPerRow];
            for (var i = 0; i < numRows; ++i)
            {
                prog.Report(i, numRows);
                var row = value.ScanlinesB[i];
                Array.Copy(row, 0, data, i * value.ElementsPerRow, row.Length);
                prog.Report(i + 1, numRows);
            }

            return new ImageData(
                value.ImgInfo.BytesPerRow / value.ImgInfo.BytesPixel,
                value.Nrows,
                value.Channels,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a PNG image.
        /// </summary>
        /// <param name="outputStream">Png bytes.</param>
        public ImageLines Translate(ImageData image, IProgress prog = null)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var imageInfo = new Hjg.Pngcs.ImageInfo(
                image.Info.Dimensions.Width,
                image.Info.Dimensions.Height,
                image.Info.BitsPerSample / image.Info.Components,
                image.Info.Components == 4);

            var imageLines = new ImageLines(
                imageInfo,
                ImageLine.ESampleType.BYTE,
                true,
                0,
                image.Info.Dimensions.Height,
                image.Info.Stride);

            for (var y = 0; y < image.Info.Dimensions.Height; ++y)
            {
                prog.Report(y, image.Info.Dimensions.Height);
                var dataIndex = y * image.Info.Stride;
                var line = imageLines.ScanlinesB[y];
                Array.Copy(image.Data, dataIndex, line, 0, image.Info.Stride);
                prog.Report(y + 1, image.Info.Dimensions.Height);
            }

            return imageLines;
        }
    }
}
