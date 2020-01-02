using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Juniper.Processes;

namespace Juniper.HTTP.Server.Administration
{
    public abstract class AbstractNetShCommand : AbstractShellCommand
    {
        private readonly string context;

        protected AbstractNetShCommand(string context)
            : base("netsh")
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (context.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(context)} cannot be an empty string.");
            }

            this.context = context;
        }

        protected override Task<int> RunAsync(IEnumerable<string> arguments)
        {
            return base.RunAsync(arguments.Prepend(context));
        }
    }
}
