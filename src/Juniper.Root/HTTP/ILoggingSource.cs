using System;

namespace Juniper.HTTP
{
    public interface ILoggingSource :
        IInfoSource,
        IWarningSource,
        IErrorSource
    { }
}
