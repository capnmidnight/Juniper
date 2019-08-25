using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.Progress;

namespace Juniper.HTTP.REST
{
    public static class AbstractRequestExt
    {
        public static async Task<ImageData> GetImage<ResponseType>(this AbstractRequest<IImageDecoder<ResponseType>, ResponseType> request, IProgress prog = null)
        {
            using (var stream = await request.GetRaw(prog))
            {
                return request.deserializer.ReadRaw(stream);
            }
        }
    }
}