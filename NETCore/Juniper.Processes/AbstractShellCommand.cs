namespace Juniper.Processes;

public abstract class AbstractShellCommand : AbstractCommand
{
    public bool AccumulateOutput { get; set; }

    protected AbstractShellCommand(string commandName)
        : base(commandName)
    {

    }
}
