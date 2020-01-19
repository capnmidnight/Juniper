using System;
using System.Threading.Tasks;
using Juniper.IO;

namespace Juniper.Imaging
{

    public static class IImageTranscoderExt
    {
        public static ToImageT Translate<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, FromImageT image)
        {
            if (transcoder is null)
            {
                throw new ArgumentNullException(nameof(transcoder));
            }

            return transcoder.Translate(image, null);
        }

        public static FromImageT Translate<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, ToImageT image)
        {
            if (transcoder is null)
            {
                throw new ArgumentNullException(nameof(transcoder));
            }

            return transcoder.Translate(image, null);
        }

        public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, FromImageT image, IProgress prog)
        {
            if (transcoder is null)
            {
                throw new ArgumentNullException(nameof(transcoder));
            }

            return Task.Run(() => transcoder.Translate(image, prog));
        }

        public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, FromImageT image)
        {
            if (transcoder is null)
            {
                throw new ArgumentNullException(nameof(transcoder));
            }

            return transcoder.TranslateAsync(image, null);
        }

        public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, ToImageT image, IProgress prog)
        {
            if (transcoder is null)
            {
                throw new ArgumentNullException(nameof(transcoder));
            }

            return Task.Run(() => transcoder.Translate(image, prog));
        }

        public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, ToImageT image)
        {
            if (transcoder is null)
            {
                throw new ArgumentNullException(nameof(transcoder));
            }

            return transcoder.TranslateAsync(image, null);
        }
    }
}
