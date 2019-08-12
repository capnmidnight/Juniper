using System.Threading.Tasks;

using Juniper.Imaging;
using Juniper.Progress;

namespace Juniper.HTTP.REST
{
    public static class AbstractRequestExt
    {
        public static async Task<ResponseType> GetImage<DecoderType, ResponseType>(this AbstractRequest<DecoderType, ResponseType> request, IProgress prog = null)
            where DecoderType : IImageDecoder<ResponseType>
        {
            using (var stream = await request.GetRaw(prog))
            {
                return request.deserializer.Read(stream);
            }
        }
    }
}
