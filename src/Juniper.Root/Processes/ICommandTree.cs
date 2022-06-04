namespace Juniper.Processes
{
    public interface ICommandTree
    {
        IEnumerable<IEnumerable<ICommand>> Tree { get; }
        ICommandTree AddCommands(params ICommand[] commands);
        ICommandTree AddCommands(IEnumerable<ICommand> commands);
    }
}
