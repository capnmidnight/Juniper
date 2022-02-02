using Juniper.Progress;

using System;
using System.Threading.Tasks;

namespace Juniper.Imaging
{

    public static class IImageCodecExt
    {
        public static ToImageT Translate<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, FromImageT image)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            return codec.Translate(image, null);
        }

        public static FromImageT Translate<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, ToImageT image)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            return codec.Translate(image, null);
        }

        public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, FromImageT image, IProgress prog)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            return Task.Run(() => codec.Translate(image, prog));
        }

        public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, FromImageT image)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            return codec.TranslateAsync(image, null);
        }

        public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, ToImageT image, IProgress prog)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            return Task.Run(() => codec.Translate(image, prog));
        }

        public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageCodec<FromImageT, ToImageT> codec, ToImageT image)
        {
            if (codec is null)
            {
                throw new ArgumentNullException(nameof(codec));
            }

            return codec.TranslateAsync(image, null);
        }
    }
}
