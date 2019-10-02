using System.IO;

namespace Juniper.IO
{
    public interface IStreamWrapper
    {
        Stream SourceStream { get; }
    }
}