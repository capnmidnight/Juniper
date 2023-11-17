namespace Juniper.Processes;

public class MessageCommand : AbstractCommand
{
    private readonly string format;
    private readonly object[] args;

    public MessageCommand(string format, params object[] args)
        : base("Message")
    {
        this.format = format;
        this.args = args;
    }

    public override Task RunAsync(CancellationToken cancellationToken)
    {
        OnInfo(string.Format(format, args));
        return Task.CompletedTask;
    }
}
