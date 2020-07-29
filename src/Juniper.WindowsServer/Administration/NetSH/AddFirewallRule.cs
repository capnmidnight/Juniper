using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public class AddFirewallRule :
        AbstractFirewallRuleCommand
    {

        public string Description
        {
            get;
            set;
        }

        public string Service
        {
            get;
            set;
        }

        public FirewallRuleDirection Direction
        {
            get;
            set;
        }

        public FirewallRuleAction Action
        {
            get;
            set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Maybe CIDRBlock shouldn't be an ICollection type")]
        public CIDRBlock RemoteBlock
        {
            get;
            set;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Usage", "CA2227:Collection properties should be read only", Justification = "Maybe CIDRBlock shouldn't be an ICollection type")]
        public CIDRBlock LocalBlock
        {
            get;
            set;
        }

        public string Program
        {
            get;
            set;
        }

        public AddFirewallRule(string name, FirewallRuleDirection direction, FirewallRuleAction action, CIDRBlock remoteBlock)
            : base("add", name)
        {
            Direction = direction;
            Action = action;
            RemoteBlock = remoteBlock;
        }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                foreach (var arg in base.Arguments)
                {
                    yield return arg;
                }

                yield return $"dir={Direction.ToString().ToLowerInvariant()}";
                yield return $"action={Action.ToString().ToLowerInvariant()}";

                if (!string.IsNullOrEmpty(Description))
                {
                    yield return $"description=\"{Description}\"";
                }

                if (!string.IsNullOrEmpty(Program))
                {
                    yield return $"program=\"{Program}\"";
                }

                if (!string.IsNullOrEmpty(Service))
                {
                    yield return $"service=\"{Service}\"";
                }

                if (LocalBlock is object)
                {
                    yield return $"localip={LocalBlock}";
                }

                if (RemoteBlock is object)
                {
                    yield return $"remoteip={RemoteBlock}";
                }
            }
        }

        protected override async Task<int> RunAsync(IEnumerable<string> arguments)
        {
            var retCode = await base.RunAsync(arguments)
                .ConfigureAwait(false);

            var message = TotalStandardOutput.Trim();
            if (!message.EndsWith("Ok.", false, CultureInfo.InvariantCulture))
            {
                throw new InvalidOperationException(message);
            }
            return retCode;
        }
    }
}
