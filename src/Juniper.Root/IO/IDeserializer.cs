using System.IO;
using System.Net;
using System.Text;

using Juniper.Progress;

namespace Juniper.IO
{
    public interface IDeserializer<out ResultT>
    {
        ResultT Deserialize(Stream stream, IProgress prog);
    }
}