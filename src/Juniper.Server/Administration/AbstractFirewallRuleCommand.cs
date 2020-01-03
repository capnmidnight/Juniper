using System;
using System.Collections.Generic;

namespace Juniper.HTTP.Server.Administration
{
    public abstract class AbstractFirewallRuleCommand :
        AbstractNetShCommand
    {
        private string name;

        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value is null)
                {
                    throw new NullReferenceException("Value cannot be null");
                }

                name = value;
            }
        }

        private readonly string command;

        protected AbstractFirewallRuleCommand(string command, string name)
        {
            this.command = command ?? throw new ArgumentNullException(nameof(command));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                yield return "advfirewall";
                yield return "firewall";
                yield return command;
                yield return "rule";

                yield return $"name=\"{Name}\"";
            }
        }
    }
}
