using System;

using Juniper.Progress;
using Juniper.Serialization;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Advanced;
using SixLabors.ImageSharp.PixelFormats;

namespace Juniper.Imaging.ImageSharp
{
    public class ImageSharpImageDataCodec : AbstractImageDataDecoder<SixLabors.ImageSharp.Image>
    {
        public ImageSharpImageDataCodec(HTTP.MediaType.Image format)
            : base(new ImageSharpCodec(format)) { }

        protected override Image TranslateTo(ImageData value, IProgress prog = null)
        {
            if (value.info.contentType != HTTP.MediaType.Image.Raw)
            {
                return Image.Load(value.data);
            }
            else if (value.info.components == 3)
            {
                return Image.LoadPixelData<Rgb24>(value.data, value.info.dimensions.width, value.info.dimensions.height);
            }
            else if (value.info.components == 4)
            {
                return Image.LoadPixelData<Rgba32>(value.data, value.info.dimensions.width, value.info.dimensions.height);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to handle number of components {value.info.components}");
            }
        }

        protected override ImageData TranslateFrom(Image image, DataSource source)
        {
            var components = image.PixelType.BitsPerPixel / 8;
            if(components == 3)
            {
                var img = (Image<Rgb24>)image;
                var span = img.GetPixelSpan();
                var data = new byte[3 * image.Width * image.Height];
                for(var i = 0; i < span.Length; ++i)
                {
                    data[i * 3 + 0] = span[i].R;
                    data[i * 3 + 1] = span[i].G;
                    data[i * 3 + 2] = span[i].B;
                }
                return new ImageData(
                    source,
                    image.Width, image.Height,
                    3,
                    HTTP.MediaType.Image.Raw,
                    data);
            }
            else if (components == 4)
            {
                var img = (Image<Rgba32>)image;
                var span = img.GetPixelSpan();
                var data = new byte[3 * image.Width * image.Height];
                for (var i = 0; i < span.Length; ++i)
                {
                    data[i * 4 + 0] = span[i].R;
                    data[i * 4 + 1] = span[i].G;
                    data[i * 4 + 2] = span[i].B;
                    data[i * 4 + 3] = span[i].A;
                }
                return new ImageData(
                    source,
                    image.Width, image.Height,
                    3,
                    HTTP.MediaType.Image.Raw,
                    data);
            }
            else
            {
                throw new NotSupportedException($"Don't know how to handle number of components {components}");
            }
        }
    }
}
