using System;

using Juniper.HTTP.REST;

namespace Juniper.Azure
{
    public abstract class AbstractAzureRequest<MediaTypeT> : AbstractRequest<MediaTypeT>
        where MediaTypeT : MediaType
    {
        private static Uri MakeURI(string region, string component)
        {
            return new Uri($"https://{region}.{component}.microsoft.com/");
        }

        protected AbstractAzureRequest(string region, string component, string path, MediaTypeT contentType)
            : base(AddPath(MakeURI(region, component), path), contentType)
        { }
    }
}
