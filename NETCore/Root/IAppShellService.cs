using Juniper.TSBuild;

namespace Juniper.AppShell;

public interface IAppShellService : IReadiable
{
    void Run();
    bool OwnsRunning { get; }
}
