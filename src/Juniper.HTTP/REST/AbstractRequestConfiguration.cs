using System;
using System.IO;

namespace Juniper.HTTP.REST
{
    public class AbstractRequestConfiguration
    {
        internal readonly DirectoryInfo cacheLocation;

        protected AbstractRequestConfiguration(DirectoryInfo cacheLocation = null)
        {
            this.cacheLocation = cacheLocation;
            cacheLocation?.Create();
        }

        protected AbstractRequestConfiguration(string cacheDirectoryName = null)
            : this(cacheDirectoryName == null ? null : new DirectoryInfo(cacheDirectoryName))
        {
        }

        public virtual Uri ModifyURI(Uri uri)
        {
            return uri;
        }
    }
}