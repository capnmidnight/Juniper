using System.IO;
using System.Threading.Tasks;

namespace Juniper.HTTP.REST
{
    public class AbstractEndpoint
    {
        internal readonly DirectoryInfo cacheLocation;

        protected AbstractEndpoint(DirectoryInfo cacheLocation = null)
        {
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractEndpoint(string cacheDirectoryName = null)
            : this(cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }
    }
}