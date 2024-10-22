using Juniper.Progress;

namespace Juniper.Imaging;

public interface IImageCodec<FromImageT, ToImageT>
{
    ToImageT Translate(FromImageT value, IProgress? prog);

    FromImageT Translate(ToImageT image, IProgress? prog);
}
