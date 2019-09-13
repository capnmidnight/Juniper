using System;
using System.IO;

namespace Juniper.HTTP.REST
{
    public class AbstractRequestConfiguration
    {
        internal readonly DirectoryInfo cacheLocation;
        public readonly Uri baseServiceURI;

        private AbstractRequestConfiguration(DirectoryInfo cacheLocation, Uri baseServiceURI)
        {
            this.baseServiceURI = baseServiceURI;
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractRequestConfiguration(Uri baseServiceURI, DirectoryInfo cacheLocation)
            : this(cacheLocation, baseServiceURI) { }

        protected AbstractRequestConfiguration(Uri baseServiceURI, string cacheDirectoryName)
            : this(new DirectoryInfo(cacheDirectoryName), baseServiceURI) { }

        protected AbstractRequestConfiguration(Uri baseServiceURI)
            : this(null, baseServiceURI) { }

        public virtual Uri ModifyURI(Uri uri)
        {
            return uri;
        }
    }
}