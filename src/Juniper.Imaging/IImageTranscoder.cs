
using Juniper.IO;

namespace Juniper.Imaging
{
    public interface IImageTranscoder<FromImageT, ToImageT>
    {
        ToImageT Translate(FromImageT value, IProgress prog);

        FromImageT Translate(ToImageT image, IProgress prog);
    }
}