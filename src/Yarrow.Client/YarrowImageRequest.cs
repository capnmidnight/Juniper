using System.Threading.Tasks;

using Juniper.Google.Maps.StreetView;
using Juniper.HTTP.REST;
using Juniper.Imaging;
using Juniper.Progress;

namespace Yarrow.Client
{
    public class YarrowImageRequest<T> : AbstractSingleRequest<T>
    {
        private PanoID pano;
        private IImageDecoder<T> decoder;

        public YarrowImageRequest(YarrowRequestConfiguration api, IImageDecoder<T> decoder)
            : base(api, decoder, "api/image", "images")
        {
            this.decoder = decoder;
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

        public async Task<T> GetJPEG(IProgress prog = null)
        {
            using (var stream = await GetRaw(prog))
            {
                return decoder.Read(stream);
            }
        }
    }
}