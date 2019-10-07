using System.Drawing;

namespace Juniper.Imaging
{
    public class ImageDataGDITranscoder : ReverseTranscoder<ImageData, Image>
    {
        public ImageDataGDITranscoder(MediaType.Image imageFormat)
            : base(new GDIImageDataTranscoder(imageFormat))
        { }
    }
}
