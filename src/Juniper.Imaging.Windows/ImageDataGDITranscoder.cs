using System.Drawing;

namespace Juniper.Imaging.Windows
{
    public class ImageDataGDITranscoder : ReverseTranscoder<ImageData, Image>
    {
        public ImageDataGDITranscoder()
            : base(new GDIImageDataTranscoder())
        { }
    }
}
