using System;

namespace Juniper.HTTP
{
    [Flags]
    public enum HttpProtocol
    {
        None,
        HTTPS,
        HTTP,
        All
    }
}
