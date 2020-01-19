//using System.Net;
//using System.Threading.Tasks;

//using NUnit.Framework;

//namespace Juniper.HTTP.Server.Administration.PowerShell.Tests
//{
//    [TestFixture]
//    public class PowerShellTests
//    {
//        private const string testAddressString1 = "192.160.0.1";
//        private static readonly IPAddress testAddress1 = IPAddress.Parse(testAddressString1);

//        private System.Management.Automation.PowerShell shell;

//        [SetUp]
//        public void Init()
//        {
//            shell = System.Management.Automation.PowerShell.Create();
//        }

//        [TearDown]
//        public void Teardown()
//        {
//            shell.Dispose();
//        }

//        private Task<bool> ExistsAsync()
//        {
//            var command = new GetFirewallRule("Test Ban");
//            return command.ExistsAsync(shell);
//        }

//        private async Task MaybeAddRuleAsync()
//        {
//            var exists = await ExistsAsync()
//                .ConfigureAwait(false);
//            if (!exists)
//            {
//                await AddRuleAsync()
//                    .ConfigureAwait(false);
//            }
//        }

//        [Test]
//        public async Task AddRuleAsync()
//        {
//            if (!HttpServer.IsAdministrator)
//            {
//                Assert.Inconclusive("This test must be ran as an administrator");
//            }

//            var command = new AddFirewallRule("Test Ban", FirewallRuleDirection.Outbound, FirewallRuleAction.Block, new CIDRBlock(testAddress1));
//            Assert.IsTrue(await command.RunAsync(shell)
//                .ConfigureAwait(false));
//        }

//        [Test]
//        public async Task DeleteRuleAsync()
//        {
//            await MaybeAddRuleAsync()
//                .ConfigureAwait(false);

//            if (!HttpServer.IsAdministrator)
//            {
//                Assert.Inconclusive("This test must be ran as an administrator");
//            }

//            var command = new DeleteFirewallRule("Test Ban");
//            Assert.IsTrue(await command.RunAsync(shell)
//                .ConfigureAwait(false));
//        }

//        [Test]
//        public async Task RuleExistsAsync()
//        {
//            await MaybeAddRuleAsync()
//                .ConfigureAwait(false);

//            Assert.IsTrue(await ExistsAsync()
//                .ConfigureAwait(false));
//        }

//        [Test]
//        public async Task GetRulesAsync()
//        {
//            await MaybeAddRuleAsync()
//                .ConfigureAwait(false);

//            var command = new GetFirewallRule("Test Ban");
//            var blocks = await command.GetRangesAsync(shell)
//                .ConfigureAwait(false);
//            Assert.IsTrue(blocks.Length > 0);
//            Assert.AreEqual(testAddress1, blocks[0].Start);
//        }
//    }
//}
