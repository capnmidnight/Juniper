using System;
using Juniper.Logging;

namespace Juniper.Logging
{
    public interface IErrorSource
    {
        event EventHandler<Exception> Error;
    }
}
