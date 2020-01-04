using System.IO;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer<out ResultT>
    {
        ResultT Deserialize(Stream stream, IProgress prog = null);
    }
}