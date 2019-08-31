using BitMiracle.LibJpeg;

namespace Juniper.Imaging.LibJpegNET
{
    public class LibJpegNETDecoder : CompositeImageFactory<JpegImage, ImageData>
    {
        public LibJpegNETDecoder(int quality = 100, int smoothingFactor = 1, bool progressive = false)
            : base(
                  new LibJpegNETCodec(quality, smoothingFactor, progressive),
                  new LibJpegNETImageDataTranscoder())
        { }
    }
}
