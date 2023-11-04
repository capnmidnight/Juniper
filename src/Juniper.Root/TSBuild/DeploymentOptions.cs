namespace Juniper.TSBuild
{
    public record DeploymentOptions(string HostName, string UserName, FileInfo KeyFile, string RemoteDirName, string? RemoteServiceName = null);
}
