namespace Juniper.AppShell
{

    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRazorPages();
            builder.Services.AddHostedService<WindowShellService>();

            var app = builder.Build();

            app.Urls.Add("http://localhost:9001");

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();
            app.MapRazorPages();
            app.Start();
        }
    }
}