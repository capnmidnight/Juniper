using System;

namespace Juniper.HTTP
{
    [Flags]
    public enum HttpMethods
    {
        None = 0,

        GET = 1,
        HEAD = 1 << 1,
        POST = 1 << 2,
        PUT = 1 << 3,
        DELETE = 1 << 4,
        CONNECT = 1 << 5,
        OPTIONS = 1 << 6,
        TRACE = 1 << 7,
        PATCH = 1 << 8,

        All = GET | HEAD | POST | PUT | DELETE | CONNECT | OPTIONS | TRACE | PATCH
    }
}
