namespace Juniper.Processes
{
    public interface ICommandTree
    {
        IEnumerable<IEnumerable<ICommand>> Tree { get; }
        ICommandTree AddCommands(params ICommand[] commands);
        ICommandTree AddCommands(Action<ICommandTree> addCommands);
        ICommandTree AddCommands(IEnumerable<ICommand> commands);
        ICommandTree AddMessage(string format, params object[] args);
    }
}
