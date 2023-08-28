namespace Juniper.AppShell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var appBuilder = WebApplication.CreateBuilder(args);            
            var app = appBuilder.AddAppShell<WPF.AppShellFactory>();
            app.Start();
        }
    }
}