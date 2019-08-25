using Juniper.Progress;
using Juniper.Serialization;

namespace Juniper.Imaging
{
    public interface IImageTranscoder<ToImageT, FromImageT>
    {
        FromImageT TranslateTo(ToImageT value, IProgress prog = null);

        ToImageT TranslateFrom(FromImageT image);
    }
}
