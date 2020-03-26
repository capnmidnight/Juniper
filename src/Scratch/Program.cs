using System;
using System.IO;
using System.Linq;
using System.Numerics;
using Juniper;
using Juniper.Imaging;
using Juniper.IO;

namespace Scratch
{
    public static class Program
    {
        public static void Main()
        {
            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var desktopDirectory = new DirectoryInfo(desktopPath);
            var imageFile = desktopDirectory
                .GetFiles("*.jpg")
                .First(f => f.Name.StartsWith("test", StringComparison.OrdinalIgnoreCase));
            using var imageStream = imageFile.OpenRead();
            var codec = new LibJpegNETCodec()
                .Pipe(new LibJpegNETImageDataTranscoder());
            var image = codec.Deserialize(imageStream);
            var images = new[]
            {
                Resize(image, 0, 0, 2),
                Resize(image, 1, 0, 2),
                Resize(image, 0, 1, 2),
                Resize(image, 1, 1, 2)
            };
            for (var i = 0; i < images.Length; ++i)
            {
                var path = Path.Combine(desktopDirectory.FullName, $"test ({i + 1}).jpg");
                codec.Serialize(path, images[i]);
            }
        }

        private static ImageData Resize(ImageData inputImage, int inputDX, int inputDY, int descale)
        {
            var z = (int)Math.Ceiling(descale / 2f);
            var c = inputImage.Info.Components;

            var inputWidth = inputImage.Info.Dimensions.Width;
            var inputHeight = inputImage.Info.Dimensions.Height;
            var inputStride = inputWidth * c;
            var inputLen = inputHeight * inputWidth * c;

            var outputWidth = inputWidth / descale - z;
            var outputHeight = inputHeight / descale - z;
            var outputStride = outputHeight * outputWidth * c;
            var outputImage = new ImageData(outputWidth, outputHeight, c);
            var outputLen = outputHeight * outputWidth * c;

            for (var inputI = 0; inputI < inputLen; inputI += c)
            {
                var inputX = (inputI / c) % inputWidth;
                var inputY = inputI / inputStride;
                var outputX = inputX / descale;
                var outputY = inputY / descale;
                var outputI = outputY * outputStride + outputX * c;
                if (outputI < outputLen - c)
                {
                    var inputColor = inputImage.GetRGB(inputI);
                    var outputColor = outputImage.GetRGB(outputI);
                    outputColor += inputColor / (descale * descale);
                    outputImage.SetRGB(outputI, outputColor);
                }
            }

            return outputImage;
        }
    }
}
