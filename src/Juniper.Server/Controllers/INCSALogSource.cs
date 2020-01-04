using System;

namespace Juniper.HTTP.Server.Controllers
{
    public interface INCSALogSource
    {
        event EventHandler<StringEventArgs> Log;
    }
}
