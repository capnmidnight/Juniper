using System.Net;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juniper.HTTP.Server.Administration.NetSH.Tests
{
    [TestClass]
    public class NetSHTests
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

        [TestMethod]
        public async Task AddRuleAsync()
        {
            var command = new AddFirewallRule("Test Ban", FirewallRuleDirection.Out, FirewallRuleAction.Block, new CIDRBlock(testAddress1));
            var retCode = await command.RunAsync()
                .ConfigureAwait(false);
            Assert.AreEqual(0, retCode);
            Assert.IsTrue(command.TotalStandardOutput.Length > 0);
        }

        [TestMethod]
        public async Task DeleteRuleAsync()
        {
            await AddRuleAsync()
                .ConfigureAwait(false);

            var command = new DeleteFirewallRule("Test Ban");
            var deleteCount = await command.RunAsync()
                .ConfigureAwait(false);
            Assert.IsTrue(deleteCount >= 1);
        }

        [TestMethod]
        public async Task RuleExistsAsync()
        {
            await AddRuleAsync()
                .ConfigureAwait(false);

            var command = new GetFirewallRule("Test Ban");
            Assert.IsTrue(await command.ExistsAsync()
                .ConfigureAwait(false));
        }

        [TestMethod]
        public async Task GetRulesAsync()
        {
            await AddRuleAsync().ConfigureAwait(false);

            var command = new GetFirewallRule("Test Ban");
            var blocks = await command.GetRangesAsync()
                .ConfigureAwait(false);
            Assert.IsTrue(blocks.Length > 0);
            Assert.AreEqual(testAddress1, blocks[0].Start);
        }
    }
}
