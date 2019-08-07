using System.Threading.Tasks;

using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Image;
using Juniper.Image.JPEG;
using Juniper.Progress;

namespace Yarrow.Client
{
    public class YarrowImageRequest : AbstractSingleRequest<ImageData>
    {
        private PanoID pano;

        public YarrowImageRequest(YarrowRequestConfiguration api)
            : base(api, new JpegFactory(), "api/image", "images")
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

        public async Task<ImageData> GetJPEG(IProgress prog = null)
        {
            using (var stream = await GetRaw(prog))
            {
                return JpegFactory.Read(stream);
            }
        }
    }
}