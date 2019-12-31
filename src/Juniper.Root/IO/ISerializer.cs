using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface ISerializer<in T>
    {
        void Serialize(Stream stream, T value, IProgress prog = null);
    }
}
