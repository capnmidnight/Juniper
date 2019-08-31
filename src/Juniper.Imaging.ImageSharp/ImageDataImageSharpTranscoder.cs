using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Juniper.Imaging.ImageSharp
{
    public class ImageDataImageSharpTranscoder<PixelT> : ReverseTranscoder<ImageData, Image<PixelT>>
        where PixelT : struct, IPixel, IPixel<PixelT>
    {
        public ImageDataImageSharpTranscoder()
            : base(new ImageSharpImageDataTranscoder<PixelT>())
        { }
    }
}