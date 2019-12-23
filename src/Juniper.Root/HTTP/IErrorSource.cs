using System;

namespace Juniper.HTTP
{
    public interface IErrorSource
    {
        event EventHandler<Exception> Error;
    }
}
