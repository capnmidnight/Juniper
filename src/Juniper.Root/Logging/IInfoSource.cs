using System;

namespace Juniper.Logging
{
    public interface IInfoSource
    {
        event EventHandler<string> Info;
    }
}
