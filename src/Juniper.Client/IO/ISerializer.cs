using System.IO;

namespace Juniper.IO
{
    public interface ISerializer<in T>
    {
        long Serialize(Stream stream, T value);
    }
}
