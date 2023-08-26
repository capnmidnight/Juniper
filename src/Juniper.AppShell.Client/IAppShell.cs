namespace Juniper.AppShell
{
    public interface IAppShell
    {
        Task<Uri> GetSourceAsync();
        Task SetSourceAsync(Uri source);

        Task CloseAsync();
    }
}