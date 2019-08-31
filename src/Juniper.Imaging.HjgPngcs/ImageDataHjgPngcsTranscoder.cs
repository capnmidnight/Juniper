
using Hjg.Pngcs;

namespace Juniper.Imaging.HjgPngcs
{
    public class ImageDataHjgPngcsTranscoder : ReverseTranscoder<ImageData, ImageLines>
    {
        public ImageDataHjgPngcsTranscoder()
            : base(new HjgPngcsImageDataTranscoder())
        { }
    }
}