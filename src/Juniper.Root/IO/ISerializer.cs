using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

using Juniper.HTTP.WebSockets;
using Juniper.Progress;

namespace Juniper.IO
{
    public interface ISerializer<in T>
    {
        void Serialize(Stream stream, T value, IProgress prog = null);
    }
}
