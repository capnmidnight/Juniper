namespace Juniper.AppShell
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            WebApplication.CreateBuilder(args)
                .ConfigureJuniperWebAppShell<WPF.AppShellFactory>()
                .Start();
        }
    }
}