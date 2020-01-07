using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration.NetSH
{
    public class ShowFirewallRule :
        AbstractFirewallRuleCommand
    {
        public bool Verbose { get; set; }
        public ShowFirewallRule(string name)
            : base("show", name)
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

                if (Verbose)
                {
                    yield return $"verbose";
                }
            }
        }

        private static readonly Regex remoteIPRangePattern = new Regex("RemoteIP:\\s+(\\d+\\.\\d+\\.\\d+\\.\\d+/\\d+)", RegexOptions.Compiled);

        protected override async Task<int> RunAsync(IEnumerable<string> arguments)
        {
            _ = await base.RunAsync(arguments)
                .ConfigureAwait(false);

            var message = TotalStandardOutput.Trim();
            if (message == "No rules match the specified criteria.")
            {
                return 0;
            }
            else if (!message.EndsWith("Ok.", false, CultureInfo.InvariantCulture))
            {
                throw new InvalidOperationException(message);
            }
            else
            {
                return 1;
            }
        }

        private static IEnumerable<CIDRBlock> FindBlocks(string message)
        {
            var matches = remoteIPRangePattern.Matches(message);
            foreach (Match match in matches)
            {
                yield return CIDRBlock.Parse(match.Groups[1].Value);
            }
        }

        public async Task<CIDRBlock[]> GetRangesAsync()
        {
            _ = await RunAsync()
                .ConfigureAwait(false);
            return FindBlocks(TotalStandardOutput.Trim()).ToArray();
        }

        public async Task<bool> ExistsAsync()
        {
            return 1 == await RunAsync()
                .ConfigureAwait(false);
        }
    }
}
