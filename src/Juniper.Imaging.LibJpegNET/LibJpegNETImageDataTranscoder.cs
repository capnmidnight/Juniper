using System;

using BitMiracle.LibJpeg;

using Juniper.Progress;

namespace Juniper.Imaging.LibJpegNET
{
    public class LibJpegNETImageDataTranscoder : AbstractCompositeImageFactory<JpegImage, ImageData>
    {
        public LibJpegNETImageDataTranscoder(int quality = 100, int smoothingFactor = 1, bool progressive = false)
            : base(new LibJpegNETCodec(quality, smoothingFactor, progressive))
        { }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public override ImageData Translate(JpegImage jpeg, IProgress prog)
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
                HTTP.MediaType.Image.Jpeg,
                data);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public override JpegImage Translate(ImageData image, IProgress prog)
        {
            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var rows = new SampleRow[image.info.dimensions.height];
            var rowBuffer = new byte[image.info.stride];
            for (var i = 0; i < image.info.dimensions.height; ++i)
            {
                copyProg.Report(i, image.info.dimensions.height);
                var imageDataIndex = i * image.info.stride;
                Array.Copy(image.data, imageDataIndex, rowBuffer, 0, rowBuffer.Length);
                rows[i] = new SampleRow(
                    rowBuffer,
                    image.info.dimensions.width,
                    ImageData.BitsPerComponent,
                    (byte)image.info.components);
                copyProg.Report(i + 1, image.info.dimensions.height);
            }

            saveProg.Report(0);
            var jpeg = new JpegImage(rows, Colorspace.RGB);
            saveProg.Report(1);
            return jpeg;
        }
    }
}