using System;
using System.Collections.Generic;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public abstract class AbstractFirewallRuleCommand :
        AbstractNetShCommand
    {
        private readonly string name;
        private readonly string command;

        protected AbstractFirewallRuleCommand(string command, string name)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            this.name = name ?? throw new ArgumentNullException(nameof(name));
        }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                yield return "advfirewall";
                yield return "firewall";
                yield return command;
                yield return "rule";
                yield return $"name=\"{name}\"";
            }
        }
    }
}
