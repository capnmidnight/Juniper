using Juniper.Progress;

namespace Juniper.Imaging
{
    public interface IImageTranscoder<ToImageT, FromImageT>
    {
        FromImageT TranslateTo(ToImageT value, IProgress prog = null);

        ToImageT TranslateFrom(FromImageT image, IProgress prog = null);
    }
}
