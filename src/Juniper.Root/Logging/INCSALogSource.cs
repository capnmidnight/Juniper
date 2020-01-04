using System;

namespace Juniper.Logging
{
    public interface INCSALogSource
    {
        event EventHandler<StringEventArgs> Log;
    }
}
