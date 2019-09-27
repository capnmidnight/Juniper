using System;
using System.IO;
using Juniper.HTTP.REST;

namespace Juniper.Azure
{
    public class AbstractAzureRequest : AbstractRequest
    {
        private static Uri MakeURI(string region, string component)
        {
            return new Uri($"https://{region}.{component}.microsoft.com/");
        }

        public AbstractAzureRequest(string region, string component, string path, DirectoryInfo cacheLocation)
            : base(AddPath(MakeURI(region, component), path), cacheLocation)
        { }
    }
}
