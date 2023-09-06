#nullable enable
namespace Juniper.Processes
{
    public abstract class AbstractCommand : ICommand
    {
        public string CommandName { get; protected set; }

        public event EventHandler<StringEventArgs>? Info;
        public event EventHandler<StringEventArgs>? Warning;
        public event EventHandler<ErrorEventArgs>? Err;

        protected AbstractCommand(string commandName)
        {
            CommandName = commandName;
        }

        public abstract Task RunAsync(CancellationToken cancellationToken);

        public async Task RunSafeAsync(CancellationToken cancellationToken)
        {
            try
            {
                await RunAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                OnError(ex);
            }
        }

        protected virtual void OnInfo(string message)
        {
            Info?.Invoke(this, new StringEventArgs(message));
        }

        protected virtual void OnWarning(string message)
        {
            Warning?.Invoke(this, new StringEventArgs(message));
        }

        protected virtual void OnError(Exception exp)
        {
            Err?.Invoke(this, new ErrorEventArgs(exp));
        }
    }
}
