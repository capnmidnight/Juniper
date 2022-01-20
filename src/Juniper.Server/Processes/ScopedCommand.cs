namespace Juniper.Processes
{
    public class ScopedCommand : IScopedShellCommand
    {
        private readonly ILogger<ICommand> logger;

        public ScopedCommand(ILogger<ICommand> logger)
        {
            this.logger = logger;
        }

        private void Command_Info(object sender, StringEventArgs e)
        {
            if (sender is ICommand proc)
            {
                logger.LogInformation("({LastCommand}): {Message}", proc.CommandName, e.Value);
            }
        }

        private void Command_Warning(object sender, StringEventArgs e)
        {
            if (sender is ICommand proc)
            {
                logger.LogWarning("({LastCommand}): {Message}", proc.CommandName, e.Value);
            }
        }

        private void Command_Error(object sender, ErrorEventArgs e)
        {
            if (sender is ICommand proc)
            {
                logger.LogError(e.Value, "({LastCommand}): {Message}", proc.CommandName, e.Value.Message);
            }
        }

        public async Task RunAsync(ICommand command, CancellationToken? token)
        {
            command.Info += Command_Info;
            command.Warning += Command_Warning;
            command.Err += Command_Error;
            try
            {
                await command.RunAsync(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "({LastCommand}): {Message}", command.CommandName, ex.Message);
            }
            finally
            {
                command.Info -= Command_Info;
                command.Warning -= Command_Warning;
                command.Err -= Command_Error;
            }
        }
    }
}
