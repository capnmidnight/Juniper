using System.Drawing;
using Juniper.HTTP;

namespace Juniper.Imaging.Windows
{
    public class ImageDataGDITranscoder : ReverseTranscoder<ImageData, Image>
    {
        public ImageDataGDITranscoder(MediaType.Image imageFormat)
            : base(new GDIImageDataTranscoder(imageFormat))
        { }
    }
}
