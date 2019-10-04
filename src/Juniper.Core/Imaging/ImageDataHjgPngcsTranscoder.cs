
using Hjg.Pngcs;

namespace Juniper.Imaging
{
    public class ImageDataHjgPngcsTranscoder : ReverseTranscoder<ImageData, ImageLines>
    {
        public ImageDataHjgPngcsTranscoder()
            : base(new HjgPngcsImageDataTranscoder())
        { }
    }
}