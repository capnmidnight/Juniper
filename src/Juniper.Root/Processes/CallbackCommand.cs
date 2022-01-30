using System;
using System.Threading;
using System.Threading.Tasks;

namespace Juniper.Processes
{
    public class CallbackCommand : AbstractCommand
    {
        private readonly Action act;

        public CallbackCommand(Action act)
        {
            this.act = act;
        }

        public override Task RunAsync(CancellationToken? token = null)
        {
            act();
            return Task.CompletedTask; ;
        }
    }
}
