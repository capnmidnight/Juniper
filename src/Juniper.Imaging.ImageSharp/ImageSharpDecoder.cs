using Juniper.HTTP;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Juniper.Imaging.ImageSharp
{
    public class ImageSharpDecoder<PixelT> : CompositeImageFactory<Image<PixelT>, ImageData>
        where PixelT : struct, IPixel, IPixel<PixelT>
    {
        public ImageSharpDecoder(MediaType.Image format)
            : base(new ImageSharpCodec<PixelT>(format), new ImageSharpImageDataTranscoder<PixelT>())
        { }
    }
}
