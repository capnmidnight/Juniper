using BitMiracle.LibJpeg;

using Juniper.Progress;

using System;

namespace Juniper.Imaging
{
    public class JpegCodec : IImageCodec<JpegImage, ImageData>
    {
        private readonly bool padAlpha;

        public JpegCodec(bool padAlpha = false)
        {
            this.padAlpha = padAlpha;
        }

        /// <summary>
        /// Decodes a raw file buffer of JPEG data into raw image buffer, with width and height saved.
        /// </summary>
        /// <param name="imageStream">Jpeg bytes.</param>
        public ImageData Translate(JpegImage value, IProgress? prog = null)
        {
            if (value is null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var width = value.Width;
            var height = value.Height;

            var inputComponents = value.ComponentsPerSample;
            var inputStride = width * inputComponents;

            var outputComponents = padAlpha ? inputComponents + 1 : inputComponents;
            var outputStride = width * outputComponents;
            var outputData = new byte[height * outputStride];

            for (var y = 0; y < height; ++y)
            {
                prog?.Report(y, height);
                var row = value.GetRow(y);
                var inputData = row.ToBytes();
                if (inputStride == outputStride)
                {
                    var outputIndex = y * outputStride;
                    Array.Copy(inputData, 0, outputData, outputIndex, inputStride);
                }
                else
                {
                    for (int inputIndex = 0, outputIndex = y * outputStride;
                        inputIndex < inputData.Length;
                        inputIndex += inputComponents, outputIndex += outputComponents)
                    {
                        for (var c = 0; c < inputComponents; ++c)
                        {
                            outputData[outputIndex + c] = inputData[inputIndex + c];
                        }
                        outputData[outputIndex + inputComponents] = byte.MaxValue;
                    }
                }
                prog?.Report(y + 1, height);
            }

            return new ImageData(
                width,
                height,
                outputComponents,
                outputData);
        }

        /// <summary>
        /// Encodes a raw file buffer of image data into a JPEG image.
        /// </summary>
        /// <param name="outputStream">Jpeg bytes.</param>
        public JpegImage Translate(ImageData image, IProgress? prog = null)
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image));
            }

            var subProgs = prog.Split("Copying", "Saving");
            var copyProg = subProgs[0];
            var saveProg = subProgs[1];
            var imageData = image.GetData();
            var rows = new SampleRow[image.Info.Dimensions.Height];
            var components = (byte)Math.Min(image.Info.Components, 3);
            var rowBuffer = new byte[image.Info.Dimensions.Width * components];
            for (var y = 0; y < image.Info.Dimensions.Height; ++y)
            {
                copyProg.Report(y, image.Info.Dimensions.Height);
                for (var x = 0; x < image.Info.Dimensions.Width; ++x)
                {
                    var imageDataIndex = (y * image.Info.Stride) + (x * image.Info.Components);
                    var rowIndex = x * components;
                    Array.Copy(imageData, imageDataIndex, rowBuffer, rowIndex, components);
                }

                rows[y] = new SampleRow(
                    rowBuffer,
                    image.Info.Dimensions.Width,
                    Units.Bits.PER_BYTE,
                    components);
                copyProg.Report(y + 1, image.Info.Dimensions.Height);
            }

            saveProg.Report(0);
            var jpeg = new JpegImage(rows, Colorspace.RGB);
            saveProg.Report(1);
            return jpeg;
        }
    }
}