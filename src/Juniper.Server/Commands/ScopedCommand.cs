using Juniper.Processes;

namespace Juniper.Services
{
    public interface IScopedShellCommand
    {
        Task RunAsync(ICommand command, CancellationToken? token);
    }

    public class ScopedCommand : IScopedShellCommand
    {
        private readonly ILogger<ICommand> logger;

        public ScopedCommand(ILogger<ICommand> logger)
        {
            this.logger = logger;
        }

        private void Command_Info(object sender, StringEventArgs e)
        {
            if (sender is ITasker proc)
            {
                logger.LogInformation("({LastCommand}): {Message}", proc.CommandName, e.Value);
            }
        }

        private void Command_Warning(object sender, StringEventArgs e)
        {
            if (sender is ITasker proc)
            {
                logger.LogWarning("({LastCommand}): {Message}", proc.CommandName, e.Value);
            }
        }

        private void Command_Error(object sender, ErrorEventArgs e)
        {
            if (sender is ITasker proc)
            {
                logger.LogError(e.Value, "({LastCommand}): {Message}", proc.CommandName, e.Value.Message);
            }
        }

        public async Task RunAsync(ICommand command, CancellationToken? token)
        {
            using var task = command.CreateTask();
            task.Info += Command_Info;
            task.Warning += Command_Warning;
            task.Err += Command_Error;
            try
            {
                await task.RunAsync(token);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "({LastCommand}): {Message}", task.CommandName, ex.Message);
            }
            finally
            {
                task.Info -= Command_Info;
                task.Warning -= Command_Warning;
                task.Err -= Command_Error;
            }
        }
    }
}
