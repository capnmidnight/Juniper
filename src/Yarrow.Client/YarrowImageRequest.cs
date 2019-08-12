using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Imaging;

namespace Yarrow.Client
{
    public class YarrowImageRequest<T> : AbstractRequest<IImageDecoder<T>, T>
    {
        private PanoID pano;
        private int fov;
        private int heading;
        private int pitch;

        public YarrowImageRequest(YarrowRequestConfiguration api, IImageDecoder<T> decoder)
            : base(api, decoder, "api/image", "images")
        {
            SetContentType("image/jpeg", "jpeg");
        }

        public PanoID Pano
        {
            get { return pano; }
            set
            {
                pano = value;
                SetQuery(nameof(pano), (string)pano);
            }
        }

        public int Heading
        {
            get { return heading; }
            set
            {
                heading = value;
                SetQuery(nameof(heading), heading);
            }
        }

        public int Pitch
        {
            get { return pitch; }
            set
            {
                pitch = value;
                SetQuery(nameof(pitch), pitch);
            }
        }

        public int FOV
        {
            get { return fov; }
            set
            {
                fov = value;
                SetQuery(nameof(fov), fov);
            }
        }
    }
}