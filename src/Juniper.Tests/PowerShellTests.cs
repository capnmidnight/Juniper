using System.Net;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.HTTP.Server.Administration.PowerShell.Tests
{
    [TestClass]
    public class PowerShellTests
    {
        private const string testAddressString0 = "192.160.0.0";
        private static readonly IPAddress testAddress0 = IPAddress.Parse(testAddressString0);

        private const string testAddressString1 = "192.160.0.1";
        private static readonly IPAddress testAddress1 = IPAddress.Parse(testAddressString1);
        private const int testBitMask1 = 32;
        private static readonly string testBlock1 = testAddressString1 + "/" + testBitMask1;

        private const string testAddressString2 = "192.160.0.2";
        private static readonly IPAddress testAddress2 = IPAddress.Parse(testAddressString2);

        private const string testAddressString3 = "192.160.0.3";
        private static readonly IPAddress testAddress3 = IPAddress.Parse(testAddressString3);

        private System.Management.Automation.PowerShell shell;

        [TestInitialize]
        public void Init()
        {
            shell = System.Management.Automation.PowerShell.Create();
        }

        [TestCleanup]
        public void Teardown()
        {
            shell.Dispose();
        }

        private Task<bool> ExistsAsync()
        {
            var command = new GetFirewallRule("Test Ban");
            return command.ExistsAsync(shell);
        }

        private async Task MaybeAddRuleAsync()
        {
            var exists = await ExistsAsync()
                .ConfigureAwait(false);
            if (!exists)
            {
                await AddRuleAsync()
                    .ConfigureAwait(false);
            }
        }

        [TestMethod]
        public async Task AddRuleAsync()
        {
            var command = new AddFirewallRule("Test Ban", FirewallRuleDirection.Outbound, FirewallRuleAction.Block, new CIDRBlock(testAddress1));
            Assert.IsTrue(await command.RunAsync(shell)
                .ConfigureAwait(false));
        }

        [TestMethod]
        public async Task DeleteRuleAsync()
        {
            await MaybeAddRuleAsync()
                .ConfigureAwait(false);

            var command = new DeleteFirewallRule("Test Ban");
            Assert.IsTrue(await command.RunAsync(shell)
                .ConfigureAwait(false));
        }

        [TestMethod]
        public async Task RuleExistsAsync()
        {
            await MaybeAddRuleAsync()
                .ConfigureAwait(false);

            Assert.IsTrue(await ExistsAsync()
                .ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetRulesAsync()
        {
            await MaybeAddRuleAsync()
                .ConfigureAwait(false);

            var command = new GetFirewallRule("Test Ban");
            var blocks = await command.GetRangesAsync(shell)
                .ConfigureAwait(false);
            Assert.IsTrue(blocks.Length > 0);
            Assert.AreEqual(testAddress1, blocks[0].Start);
        }
    }
}
