using System.Threading.Tasks;

using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.Image.JPEG;

namespace Yarrow.Client
{
    public class YarrowImageRequest : AbstractSingleRequest<ImageData>
    {
        private PanoID pano;

        public YarrowImageRequest(YarrowRequestConfiguration api)
            : base(api, new JpegFactory(), "api/image", "metadata")
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

        public async Task<ImageData> GetJPEG()
        {
            using (var stream = await GetRaw())
            {
                return JpegFactory.Read(stream);
            }
        }
    }
}