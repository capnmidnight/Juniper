using System;
using System.IO;
using Juniper.HTTP.REST;

namespace Yarrow.Client
{
    public class YarrowRequestConfiguration : AbstractRequestConfiguration
    {
        public YarrowRequestConfiguration(Uri baseServiceURI, DirectoryInfo cacheLocation = null) : base(baseServiceURI, cacheLocation)
        {
        }

        public YarrowRequestConfiguration(Uri baseServiceURI, string cacheDirectoryName = null) : base(baseServiceURI, cacheDirectoryName)
        {
        }
    }
}