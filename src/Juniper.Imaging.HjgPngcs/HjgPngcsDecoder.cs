using Hjg.Pngcs;

namespace Juniper.Imaging.HjgPngcs
{
    public class HjgPngcsDecoder : CompositeImageFactory<ImageLines, ImageData>
    {
        public HjgPngcsDecoder(int compressionLevel = 9, int IDATMaxSize = 0x1000)
            : base(new HjgPngcsCodec(compressionLevel, IDATMaxSize), new HjgPngcsImageDataTranscoder())
        { }
    }
}
