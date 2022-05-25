using System.Text.RegularExpressions;

namespace Juniper.Processes
{
    public abstract class AbstractShellCommand : AbstractCommand
    {
        private readonly Dictionary<Regex, List<ICommand>> stdOutputCommands = new();
        private readonly Dictionary<Regex, List<ICommand>> stdErrorCommands = new();

        public bool AccumulateOutput { get; set; }

        protected AbstractShellCommand(string commandName)
            : base(commandName)
        {

        }

        public AbstractShellCommand OnStandardOutput(Regex pattern, params ICommand[] commands)
        {
            if (!stdOutputCommands.ContainsKey(pattern))
            {
                stdOutputCommands.Add(pattern, new List<ICommand>());
            }

            stdOutputCommands[pattern].AddRange(commands);
            return this;
        }

        public AbstractShellCommand OnStandardOutput(Regex pattern, IEnumerable<ICommand> commands)
        {
            return OnStandardOutput(pattern, commands.ToArray());
        }

        public AbstractShellCommand OnStandardOutput(Regex pattern, Action act)
        {
            return OnStandardOutput(pattern, new CallbackCommand(act));
        }

        public AbstractShellCommand OnStandardError(Regex pattern, params ICommand[] commands)
        {
            if (!stdErrorCommands.ContainsKey(pattern))
            {
                stdErrorCommands.Add(pattern, new List<ICommand>());
            }

            stdErrorCommands[pattern].AddRange(commands);
            return this;
        }

        public AbstractShellCommand OnStandardError(Regex pattern, IEnumerable<ICommand> commands)
        {
            return OnStandardError(pattern, commands.ToArray());
        }

        public AbstractShellCommand OnStandardError(Regex pattern, Action act)
        {
            return OnStandardError(pattern, new CallbackCommand(act));
        }

        private async Task ProcessCommands(Dictionary<Regex, List<ICommand>> outputCommands, string line)
        {
            foreach (var (pattern, commands) in outputCommands)
            {
                if (pattern.IsMatch(line))
                {
                    await Task.WhenAll(commands.Select(RunCommand));
                }
            }
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

        protected override void OnInfo(string message)
        {
            base.OnInfo(message);
            _ = ProcessCommands(stdOutputCommands, message);
        }

        protected override void OnWarning(string message)
        {
            base.OnWarning(message);
            _ = ProcessCommands(stdErrorCommands, message);
        }
    }
}
