namespace Juniper.Processes;

public class CallbackCommand : AbstractCommand
{
    private readonly Action act;

    public CallbackCommand(Action act)
        : base("Callback")
    {
        this.act = act;
    }

    public override Task RunAsync(CancellationToken cancellationToken)
    {
        act();
        return Task.CompletedTask; ;
    }
}
