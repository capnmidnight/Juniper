using System;

using BitMiracle.LibJpeg;

using Juniper.Progress;

namespace Juniper.Imaging
{
    public class LibJpegNETImageDataTranscoder : IImageTranscoder<JpegImage, ImageData>
    {
        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public ImageData Translate(JpegImage jpeg, IProgress prog)
        {
            var stride = jpeg.Width * jpeg.ComponentsPerSample;
            var numRows = jpeg.Height;
            var data = new byte[numRows * stride];
            for (var i = 0; i < jpeg.Height; ++i)
            {
                prog.Report(i, jpeg.Height);
                var row = jpeg.GetRow(i);
                Array.Copy(row.ToBytes(), 0, data, i * stride, stride);
                prog.Report(i + 1, jpeg.Height);
            }

            return new ImageData(
                jpeg.Width,
                jpeg.Height,
                jpeg.ComponentsPerSample,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public JpegImage Translate(ImageData image, IProgress prog)
        {
            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var rows = new SampleRow[image.info.dimensions.height];
            var rowBuffer = new byte[image.info.stride];
            var components = (byte)Math.Min(image.info.components, 3);
            for (var y = 0; y < image.info.dimensions.height; ++y)
            {
                copyProg.Report(y, image.info.dimensions.height);
                for (var x = 0; x < image.info.dimensions.width; ++x)
                {
                    var imageDataIndex = (y * image.info.stride) + (x * image.info.components);
                    var rowIndex = x * components;
                    Array.Copy(image.data, imageDataIndex, rowBuffer, rowIndex, components);
                }

                rows[y] = new SampleRow(
                    rowBuffer,
                    image.info.dimensions.width,
                    ImageData.BitsPerComponent,
                    components);
                copyProg.Report(y + 1, image.info.dimensions.height);
            }

            saveProg.Report(0);
            var jpeg = new JpegImage(rows, Colorspace.RGB);
            saveProg.Report(1);
            return jpeg;
        }
    }
}