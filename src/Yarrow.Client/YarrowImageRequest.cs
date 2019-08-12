using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Imaging;

namespace Yarrow.Client
{
    public class YarrowImageRequest<T> : AbstractRequest<IImageDecoder<T>, T>
    {
        private PanoID pano;

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
    }
}