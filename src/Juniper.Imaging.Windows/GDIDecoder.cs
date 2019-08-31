using Juniper.HTTP;

namespace Juniper.Imaging.Windows
{
    public class GDIDecoder : CompositeImageFactory<System.Drawing.Image, ImageData>
    {
        public GDIDecoder(MediaType.Image format)
            : base(new GDICodec(format), new GDIImageDataTranscoder())
        { }
    }
}
