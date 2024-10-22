namespace Juniper.TSBuild;

public interface IReadiable
{
    Task Ready { get; }
    Task Complete { get; }
}