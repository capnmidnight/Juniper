namespace Juniper.AppShell
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddAppShell();

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