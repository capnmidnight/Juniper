//using System;
//using System.Threading.Tasks;

//namespace Juniper.HTTP.Server.Administration.PowerShell
//{
//    public class DeleteFirewallRule
//    {
//        public string Name
//        {
//            get;
//            set;
//        }

//        public DeleteFirewallRule(string name)
//        {
//            Name = name;
//        }

//        public Task<bool> RunAsync()
//        {
//            using var shell = System.Management.Automation.PowerShell.Create();
//            return RunAsync(shell);
//        }

//        public async Task<bool> RunAsync(System.Management.Automation.PowerShell shell)
//        {
//            if (shell is null)
//            {
//                throw new ArgumentNullException(nameof(shell));
//            }

//            shell.Commands.Clear();

//            shell.AddCommand("Remove-NetFirewallRule")
//                .AddParameter("-DisplayName", Name);

//            var result = await shell
//                .BeginInvoke()
//                .AsAsync()
//                .ConfigureAwait(false);

//            using var data = shell.EndInvoke(result);

//            return !shell.HadErrors;
//        }
//    }
//}
