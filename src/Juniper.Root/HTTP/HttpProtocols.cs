using System;

namespace Juniper.HTTP
{
    [Flags]
    public enum HttpProtocols
    {
        None,

        HTTPS,
        HTTP,

        All = HTTPS | HTTP
    }
}
