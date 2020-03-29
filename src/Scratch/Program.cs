using System;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Metadata.W3cXsd2001;
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
            var spaces = new ColorSpace[]
            {
                ColorSpace.HSI,
                ColorSpace.HSL,
                ColorSpace.HSV,
                ColorSpace.RGB,
                ColorSpace.YCH_Adobe,
                ColorSpace.YCH_HDR,
                ColorSpace.YCH_SDTV,
                ColorSpace.YCH_sRGB
            };

            for (var s = 0; s < spaces.Length; ++s)
            {
                var images = new[]
                {
                    Resize(image, 0, 0, 3, spaces[s]),
                    Resize(image, 1, 0, 3, spaces[s]),
                    Resize(image, 0, 1, 3, spaces[s]),
                    Resize(image, 1, 1, 3, spaces[s])
                };

                for (var i = 0; i < images.Length; ++i)
                {
                    var path = Path.Combine(desktopDirectory.FullName, $"test ({s * images.Length + i + 1}).jpg");
                    codec.Serialize(path, images[i]);
                }
            }
        }

        private static ImageData Resize(ImageData inputImage, int inputDX, int inputDY, int descale, ColorSpace space)
        {
            var c = inputImage.Info.Components;

            var inputWidth = inputImage.Info.Dimensions.Width;
            var inputHeight = inputImage.Info.Dimensions.Height;

            var outputWidth = inputWidth / descale;
            var outputHeight = inputHeight / descale;
            var outputImage = new ImageData(outputWidth, outputHeight, c);

            var xRatio = (float)outputWidth / inputWidth;
            var yRatio = (float)outputHeight / inputHeight;

            for(var inputY = inputDY; inputY < inputHeight; ++inputY)
            {
                var oY = inputY * yRatio;
                var outputTop = (int)oY;
                var outputBottom = (int)(oY + yRatio);
                var dy = outputBottom - outputTop;
                for (var inputX = inputDX; inputX < inputWidth; ++inputX)
                {
                    var oX = inputX * xRatio;
                    var outputLeft = (int)oX;
                    var outputRight = (int)(oX + xRatio);
                    var dx = outputRight - outputLeft;

                    var inputP = inputImage.GetRGB(inputX, inputY).ConvertTo(space);
                    for (var outputY = outputTop; outputY <= outputBottom && outputY < outputHeight; ++outputY)
                    {
                        var sY = dy == 0
                            ? yRatio
                            : outputY == outputTop
                                ? outputBottom - oY
                                : oY + yRatio - outputBottom;
                        for(var outputX = outputLeft; outputX <= outputRight && outputX < outputWidth; ++outputX)
                        {
                            var sX = dx == 0
                                ? xRatio
                                : outputX == outputLeft
                                    ? outputRight - oX
                                    : oX + xRatio - outputRight;
                            var s = sY * sX;
                            var outputP = outputImage.GetRGB(outputX, outputY).ConvertTo(space);
                            outputP += s * inputP;
                            outputImage.SetRGB(outputX, outputY, outputP);
                        }
                    }
                }
            }

            return outputImage;
        }
    }
}
