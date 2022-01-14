using Juniper.Processes;

namespace Juniper.Services
{
    public interface ICommand
    {
        ITasker CreateTask();
    }
}
