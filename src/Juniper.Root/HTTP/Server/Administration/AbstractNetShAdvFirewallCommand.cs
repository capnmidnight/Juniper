using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration
{
    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="Juniper.HTTP.Server.Administration.AbstractNetShCommand" />
    /// <remarks>
    /// https://docs.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2008-R2-and-2008/dd734783%28v%3dws.10%29
    ///
    /// netsh advfirewall firewall add rule name="IP Block" dir=in interface=any action = block remoteip=xxx.xxx.xxx.xxx/32
    /// </remarks>
    public abstract class AbstractNetShAdvFirewallCommand :
        AbstractNetShCommand
    {
        private readonly string subCommand;

        protected AbstractNetShAdvFirewallCommand(string subCommand)
            : base("advfirewall")
        {
            if (subCommand is null)
            {
                throw new ArgumentNullException(nameof(subCommand));
            }

            if (subCommand.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(subCommand)} cannot be an empty string.");
            }

            this.subCommand = subCommand;
        }

        protected override Task<int> RunAsync(IEnumerable<string> arguments)
        {
            return base.RunAsync(arguments.Prepend(subCommand));
        }
    }
}
