using System;

using Juniper.Progress;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Juniper.Imaging.ImageSharp
{
    public class ImageSharpImageDataTranscoder<PixelT> : IImageTranscoder<Image<PixelT>, ImageData>
        where PixelT : struct, IPixel, IPixel<PixelT>
    {
        public ImageData TranslateTo(Image<PixelT> image, IProgress prog = null)
        {
            var components = image.PixelType.BitsPerPixel / 8;
            if (components != 3 && components != 4)
            {
                throw new NotSupportedException($"Don't know how to handle number of components {components}");
            }
            else
            {
                var j = 0;
                var data = new byte[components * image.Width * image.Height];
                if (components == 3)
                {
                    var img = image as Image<Rgb24>;
                    var span = img.GetPixelSpan();
                    for (var i = 0; i < span.Length; ++i)
                    {
                        prog?.Report(i, span.Length);
                        data[j++] = span[i].R;
                        data[j++] = span[i].G;
                        data[j++] = span[i].B;
                        prog?.Report(i + 1, span.Length);
                    }
                }
                else if (components == 4)
                {
                    var img = image as Image<Rgba32>;
                    var span = img.GetPixelSpan();
                    for (var i = 0; i < span.Length; ++i)
                    {
                        prog?.Report(i, span.Length);
                        data[j++] = span[i].R;
                        data[j++] = span[i].G;
                        data[j++] = span[i].B;
                        data[j++] = span[i].A;
                        prog?.Report(i + 1, span.Length);
                    }
                }

                return new ImageData(
                    image.Width,
                    image.Height,
                    components,
                    HTTP.MediaType.Image.Raw,
                    data);
            }
        }

        public Image<PixelT> TranslateFrom(ImageData value, IProgress prog = null)
        {
            prog?.Report(0);
            Image<PixelT> img;
            if (value.contentType != HTTP.MediaType.Image.Raw)
            {
                img = Image.Load<PixelT>(value.data);
            }
            else if (value.info.components == 3)
            {
                img = Image.LoadPixelData<PixelT>(value.data, value.info.dimensions.width, value.info.dimensions.height);
            }
            else if (value.info.components == 4)
            {
                img = Image.LoadPixelData<PixelT>(value.data, value.info.dimensions.width, value.info.dimensions.height);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to handle number of components {value.info.components}");
            }
            prog?.Report(1);
            return img;
        }
    }
}
