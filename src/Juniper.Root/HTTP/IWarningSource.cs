using System;

namespace Juniper.HTTP
{

    public interface IWarningSource
    {
        event EventHandler<string> Warning;
    }
}
