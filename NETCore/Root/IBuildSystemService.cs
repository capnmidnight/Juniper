namespace Juniper.TSBuild;

public interface IBuildSystemService : IReadiable
{
    Task Started { get; }
    event EventHandler NewBuildCompleted;
}
