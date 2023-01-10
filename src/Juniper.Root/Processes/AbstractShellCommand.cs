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

        private async Task RunCommand(ICommand command)
        {
            command.Info += Command_Info;
            command.Warning += Command_Warning;
            command.Err += Command_Error;
            await command.RunSafeAsync();
            command.Info -= Command_Info;
            command.Warning -= Command_Warning;
            command.Err -= Command_Error;
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
