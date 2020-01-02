using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Juniper.HTTP.Server.Administration
{
    public abstract class AbstractNetShHttpAddCommand :
        AbstractNetShHttpCommand
    {
        private readonly string subCommand;

        protected AbstractNetShHttpAddCommand(string subCommand)
            : base("add")
        {
            if (subCommand is null)
            {
                throw new ArgumentNullException(nameof(subCommand));
            }

            if (subCommand.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(subCommand)} cannot be an empty string.");
            }

            this.subCommand = subCommand;
        }

        protected override Task<int> RunAsync(IEnumerable<string> arguments)
        {
            return base.RunAsync(arguments.Prepend(subCommand));
        }
    }
}
