using System;

namespace Juniper.Logging
{
    public interface IWarningSource
    {
        event EventHandler<string> Warning;
    }
}
