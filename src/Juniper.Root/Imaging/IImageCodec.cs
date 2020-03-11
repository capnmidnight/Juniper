using Juniper.IO;

namespace Juniper.Imaging
{
    public interface IImageCodec<T> : IFactory<T, MediaType.Image>
    {
    }

    public static class IImageCodecExt
    {
        public static IImageCodec<ToImageT> Pipe<FromImageT, ToImageT>(this IImageCodec<FromImageT> codec, IImageTranscoder<FromImageT, ToImageT> transcoder)
        {
            return new TranscoderCodec<FromImageT, ToImageT>(codec, transcoder);
        }
    }
}