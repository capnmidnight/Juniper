
using Juniper.IO;

namespace Juniper.Imaging
{
    public interface IImageProcessor<T>
    {
        int GetWidth(T img);

        int GetHeight(T img);

        int GetComponents(T img);

        T Concatenate(T[,] images, IProgress prog = null);
    }
}