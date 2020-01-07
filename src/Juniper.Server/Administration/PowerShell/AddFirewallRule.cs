using System;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration.PowerShell
{
    public class AddFirewallRule
    {
        public string Name
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

        public AddFirewallRule(string name, FirewallRuleDirection direction, FirewallRuleAction action, CIDRBlock remoteBlock)
        {
            Name = name;
            Direction = direction;
            Action = action;
            RemoteBlock = remoteBlock;
        }

        public Task<bool> RunAsync()
        {
            using var shell = System.Management.Automation.PowerShell.Create();
            return RunAsync(shell);
        }

        public async Task<bool> RunAsync(System.Management.Automation.PowerShell shell)
        {
            if (shell is null)
            {
                throw new ArgumentNullException(nameof(shell));
            }

            shell.Commands.Clear();

            shell.AddCommand("New-NetFirewallRule")
                .AddParameter("-DisplayName", Name)
                .AddParameter("-Direction", Direction.ToString())
                .AddParameter("-Action", Action.ToString())
                .AddParameter("-RemoteAddress", RemoteBlock.ToString());

            var result = await shell
                .BeginInvoke()
                .AsAsync()
                .ConfigureAwait(false);

            using var data = shell.EndInvoke(result);

            foreach (var item in data)
            {
                foreach (var prop in item.Properties)
                {
                    if (prop.Name == "Enabled")
                    {
                        return (ushort)prop.Value == 1;
                    }
                }
            }

            return false;
        }
    }
}
