using System.Threading.Tasks;

using Juniper.Progress;

namespace Juniper.Imaging
{

    public static class IImageTranscoderExt
    {
        public static ToImageT Translate<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, FromImageT image)
        {
            return transcoder.Translate(image, null);
        }

        public static FromImageT Translate<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, ToImageT image)
        {
            return transcoder.Translate(image, null);
        }

        public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, FromImageT image, IProgress prog)
        {
            return Task.Run(() => transcoder.Translate(image, prog));
        }

        public static Task<ToImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, FromImageT image)
        {
            return transcoder.TranslateAsync(image, null);
        }

        public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, ToImageT image, IProgress prog)
        {
            return Task.Run(() => transcoder.Translate(image, prog));
        }

        public static Task<FromImageT> TranslateAsync<FromImageT, ToImageT>(this IImageTranscoder<FromImageT, ToImageT> transcoder, ToImageT image)
        {
            return transcoder.TranslateAsync(image, null);
        }
    }
}
