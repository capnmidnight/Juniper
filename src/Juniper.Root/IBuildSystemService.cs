namespace Juniper.TSBuild
{
    public interface IBuildSystemService
    {
        Task Ready { get; }
    }
}
