using BitMiracle.LibJpeg;

namespace Juniper.Imaging
{
    public class ImageDataLibJpegNETTranscoder : ReverseTranscoder<ImageData, JpegImage>
    {
        public ImageDataLibJpegNETTranscoder()
            : base(new LibJpegNETImageDataTranscoder())
        { }
    }
}
