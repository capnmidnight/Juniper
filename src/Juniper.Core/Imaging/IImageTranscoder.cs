using Juniper.Progress;

namespace Juniper.Imaging
{
    public interface IImageTranscoder<FromImageT, ToImageT> : IImageCodec<ToImageT>
    {
        ToImageT Translate(FromImageT value, IProgress prog);

        FromImageT Translate(ToImageT image, IProgress prog);
    }
}
