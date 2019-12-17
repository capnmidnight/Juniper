using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Juniper.HTTP
{
    [Flags]
    public enum HttpMethod
    {
        None = 0,

        GET = 1,
        HEAD = 2,
        POST = 4,
        PUT = 8,
        DELETE = 16,
        CONNECT = 32,
        OPTIONS = 64,
        TRACE = 128,
        PATCH = 256,

        All = GET | HEAD | POST | PUT | DELETE | CONNECT | OPTIONS | TRACE | PATCH
    }
}
