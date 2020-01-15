using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration.PowerShell
{
    public class GetFirewallRule
    {
        public string Name { get; set; }

        public GetFirewallRule(string name)
        {
            Name = name;
        }

        public Task<bool> ExistsAsync()
        {
            using var shell = System.Management.Automation.PowerShell.Create();
            return ExistsAsync(shell);
        }

        public async Task<bool> ExistsAsync(System.Management.Automation.PowerShell shell)
        {
            if (shell is null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            shell.Commands.Clear();

            shell.AddCommand("Get-NetFirewallRule")
                .AddParameter("-DisplayName", Name);

            var result = await shell
                .BeginInvoke()
                .AsAsync()
                .ConfigureAwait(false);

            using var data = shell.EndInvoke(result);

            return data.Count > 0;
        }

        public Task<CIDRBlock[]> GetRangesAsync()
        {
            using var shell = System.Management.Automation.PowerShell.Create();
            return GetRangesAsync(shell);
        }
        public async Task<CIDRBlock[]> GetRangesAsync(System.Management.Automation.PowerShell shell)
        {
            if (shell is null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            shell.Commands.Clear();

            shell.AddCommand("Get-NetFirewallRule")
                .AddParameter("-DisplayName", Name)
                .AddCommand("Get-NetFirewallAddressFilter");

            var result = await shell
                .BeginInvoke()
                .AsAsync()
                .ConfigureAwait(false);

            using var data = shell.EndInvoke(result);

            return EnumerateBlocks(data)
                .ToArray();
        }

        private static IEnumerable<CIDRBlock> EnumerateBlocks(PSDataCollection<PSObject> data)
        {
            foreach (var item in data)
            {
                foreach (var prop in item.Properties)
                {
                    if (prop.Name == "RemoteAddress")
                    {
                        var values = (string[])prop.Value;
                        foreach (var value in values)
                        {
                            yield return CIDRBlock.Parse(value);
                        }
                    }
                }
            }
        }
    }
}
