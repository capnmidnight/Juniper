using System.IO;

namespace Juniper.IO
{
    public interface IDataSource
    {
        Stream GetStream(string fileName);
    }
}