using System.IO;

namespace Juniper.IO
{
    public interface IDeserializer<out ResultT>
    {
        ResultT Deserialize(Stream stream);
    }
}