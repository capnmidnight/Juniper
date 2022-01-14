namespace Juniper.Services
{
    public interface IShellCommandTree
    {
        IEnumerable<IEnumerable<ICommand>> Tree { get; }
        IShellCommandTree AddCommands(params ICommand[] commands);
    }

    public class CommandTree : IShellCommandTree
    {
        private readonly List<ICommand[]> commandTree = new();

        public IEnumerable<IEnumerable<ICommand>> Tree => commandTree;

        public IShellCommandTree AddCommands(params ICommand[] commands)
        {
            commandTree.Add(commands);
            return this;
        }
    }
}
