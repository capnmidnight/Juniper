using Juniper.Progress;

namespace Juniper.Imaging
{
    public interface IImageTranscoder<ToImageT, FromImageT>
    {
        FromImageT TranslateTo(ToImageT value, IProgress prog);

        ToImageT TranslateFrom(FromImageT image, IProgress prog);
    }

    public static class IImageTranscoderExt
    {
        public static FromImageT TranslateTo<ToImageT, FromImageT>(this IImageTranscoder<ToImageT, FromImageT> transcoder, ToImageT value)
        {
            return transcoder.TranslateTo(value, null);
        }

        public static ToImageT TranslateFrom<ToImageT, FromImageT>(this IImageTranscoder<ToImageT, FromImageT> transcoder, FromImageT value)
        {
            return transcoder.TranslateFrom(value, null);
        }
    }
}
