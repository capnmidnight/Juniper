using Juniper.Processes;

namespace Juniper.HTTP.Server.Administration
{
    public abstract class AbstractNetShCommand :
        AbstractShellCommand
    {
        protected AbstractNetShCommand()
            : base("netsh")
        { }
    }
}
