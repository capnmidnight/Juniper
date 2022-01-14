using Juniper.Processes;

namespace Juniper.Services
{
    public class ShellCommand : ICommand
    {
        private readonly string command;
        private readonly string[] args;

        public ShellCommand(string command, params string[] args)
        {
            if (command is null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            if (command.Length == 0)
            {
                throw new InvalidOperationException($"{nameof(command)} cannot be an empty string.");
            }

            this.command = command;
            this.args = args ?? Array.Empty<string>();
        }

        public ITasker CreateTask()
        {
            return new ProcessTasker(command, args);
        }
    }
}
