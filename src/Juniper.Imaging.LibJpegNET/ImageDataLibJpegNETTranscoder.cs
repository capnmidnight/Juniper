using BitMiracle.LibJpeg;

namespace Juniper.Imaging.LibJpegNET
{
    public class ImageDataLibJpegNETTranscoder : ReverseTranscoder<ImageData, JpegImage>
    {
        public ImageDataLibJpegNETTranscoder()
            : base(new LibJpegNETImageDataTranscoder())
        { }
    }
}
