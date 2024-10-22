using Juniper.Progress;

namespace Juniper.Imaging;


public static class IImageCodecExt
{
    public static ToImageT Translate<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, FromImageT image, IProgress? prog = null)
    {
        return codec.Translate(image, prog);
    }

    public static FromImageT Translate<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, ToImageT image, IProgress? prog = null)
    {
        return codec.Translate(image, prog);
    }

    public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, FromImageT image, IProgress? prog = null)
    {
        return Task.Run(() => codec.Translate(image, prog));
    }

    public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, ToImageT image, IProgress? prog = null)
    {
        if (codec is null)
        {
            throw new ArgumentNullException(nameof(codec));
        }

        return Task.Run(() => codec.Translate(image, prog));
    }
}
