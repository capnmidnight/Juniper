using Juniper.IO;

namespace Juniper.Imaging;

public interface IImageFactory<T> : IFactory<T, MediaType.Image>
{
}

public static class IImageFactoryExt
{
    public static IImageFactory<ToImageT> Pipe<FromImageT, ToImageT>(this IImageFactory<FromImageT> factory, IImageCodec<FromImageT, ToImageT> codec)
    {
        return new ImageCodec<FromImageT, ToImageT>(factory, codec);
    }
}