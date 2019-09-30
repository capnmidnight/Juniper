using System;

using Juniper.HTTP;
using Juniper.HTTP.REST;

namespace Juniper.Azure
{
    public abstract class AbstractAzureRequest : AbstractRequest
    {
        private static Uri MakeURI(string region, string component)
        {
            return new Uri($"https://{region}.{component}.microsoft.com/");
        }

        public AbstractAzureRequest(string region, string component, string path, MediaType contentType)
            : base(AddPath(MakeURI(region, component), path), contentType)
        { }
    }
}
