using System;

namespace Juniper.Logging
{
    public interface IErrorSource
    {
        event EventHandler<Exception> Error;
    }
}
