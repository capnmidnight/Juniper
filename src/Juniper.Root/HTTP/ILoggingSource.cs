using System;

namespace Juniper.HTTP
{
    public interface IInfoSource
    {
        event EventHandler<string> Info;
    }

    public interface IWarningSource
    {
        event EventHandler<string> Warning;
    }

    public interface IErrorSource
    {
        event EventHandler<string> Error;
    }

    public interface ILoggingSource :
        IInfoSource,
        IWarningSource,
        IErrorSource
    { }
}
