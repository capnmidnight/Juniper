using System;

namespace Juniper.HTTP
{
    public interface IInfoSource
    {
        event EventHandler<string> Info;
    }
}
