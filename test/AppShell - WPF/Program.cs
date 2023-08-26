namespace Juniper.AppShell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddAppShell<WPF.AppShellFactory>();

            var app = builder.Build();

            app.UseAppShell();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();
            app.Start();
        }
    }
}