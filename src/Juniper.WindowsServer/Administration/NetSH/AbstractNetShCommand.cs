using Juniper.Processes;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public abstract class AbstractNetShCommand :
        AbstractShellCommand
    {
        protected AbstractNetShCommand()
            : base("netsh")
        { }
    }
}
