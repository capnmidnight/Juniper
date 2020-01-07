using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public class DeleteFirewallRule :
        AbstractFirewallRuleCommand
    {
        public string Service
        {
            get;
            set;
        }

        public FirewallRuleDirection? Direction
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

        public DeleteFirewallRule(string name)
            : base("delete", name)
        {
        }

        protected override IEnumerable<string> Arguments
        {
            get
            {
                foreach (var arg in base.Arguments)
                {
                    yield return arg;
                }

                if (Direction is object)
                {
                    yield return $"dir={Direction.Value.ToString().ToLowerInvariant()}";
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

        private static readonly Regex deleteCountPattern = new Regex("Deleted (\\d+) rule", RegexOptions.Compiled);

        protected override async Task<int> RunAsync(IEnumerable<string> arguments)
        {
            _ = await base.RunAsync(arguments)
                .ConfigureAwait(false);

            var message = TotalStandardOutput.Trim();
            if (!message.EndsWith("Ok.", false, CultureInfo.InvariantCulture)
                && message != "No rules match the specified criteria.")
            {
                throw new InvalidOperationException(message);
            }

            var match = deleteCountPattern.Match(message);
            if (!match.Success)
            {
                return 0;
            }
            else
            {
                return int.Parse(match.Groups[1].Value, CultureInfo.InvariantCulture);
            }
        }
    }
}
