using System.IO;

namespace Juniper.Streams
{
    public interface IStreamWrapper
    {
        Stream UnderlyingStream { get; }
    }
}