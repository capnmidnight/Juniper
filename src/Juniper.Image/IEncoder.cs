using System.IO;

namespace Juniper.Image
{
    public interface IEncoder
    {
        void Encode(RawImage image, Stream outputStream, bool flipImage);
    }
}