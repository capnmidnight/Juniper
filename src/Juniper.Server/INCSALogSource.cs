using System;

namespace Juniper.HTTP.Server
{
    public interface INCSALogSource
    {
        event EventHandler<StringEventArgs> Log;
    }
}
