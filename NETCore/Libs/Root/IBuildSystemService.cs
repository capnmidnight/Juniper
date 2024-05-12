namespace Juniper.TSBuild;

public interface IBuildSystemService
{
    Task Started { get; }
    Task Ready { get; }
    event EventHandler NewBuildCompleted;
}
