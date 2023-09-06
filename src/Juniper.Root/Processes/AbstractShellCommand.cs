#nullable enable

namespace Juniper.Processes
{
    public abstract class AbstractShellCommand : AbstractCommand
    {
        public bool AccumulateOutput { get; set; }

        protected AbstractShellCommand(string commandName)
            : base(commandName)
        {

        }

        private void Command_Info(object? sender, StringEventArgs e)
        {
            OnInfo(e.Value);
        }

        private void Command_Warning(object? sender, StringEventArgs e)
        {
            OnWarning(e.Value);
        }

        private void Command_Error(object? sender, ErrorEventArgs e)
        {
            OnError(e.Value);
        }
    }
}
