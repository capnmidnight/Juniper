using System;
using System.IO;

namespace Juniper.HTTP.REST
{
    public class AbstractRequestConfiguration
    {
        internal readonly DirectoryInfo cacheLocation;
        internal readonly Uri baseServiceURI;

        protected AbstractRequestConfiguration(Uri baseServiceURI, DirectoryInfo cacheLocation = null)
        {
            this.baseServiceURI = baseServiceURI;
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractRequestConfiguration(Uri baseServiceURI, string cacheDirectoryName = null)
            : this(baseServiceURI, cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }

        public virtual Uri ModifyURI(Uri uri)
        {
            return uri;
        }
    }
}