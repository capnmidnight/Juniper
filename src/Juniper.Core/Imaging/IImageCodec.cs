using Juniper.IO;

namespace Juniper.Imaging
{
    public interface IImageCodec<T> : IFactory<T, MediaType.Image>
    {
    }
}