namespace Juniper.AppShell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseAppShell<WPF.AppShellFactory>(true);
            builder.Services.AddRazorPages();

            var app = builder.Build();

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();
            app.Start();
        }
    }
}