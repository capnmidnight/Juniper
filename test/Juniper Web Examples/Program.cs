using Juniper.Services;

namespace Juniper.Examples
{
    public static class Program
    {
        public static readonly DefaultConfiguration.PortOptions ports = new()
        {
            HttpPort = 80,
            HttpsPort = 443
        };

        public static void Main(string[] args)
        {
            CreateHostBuilder(args)
                .Build()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            return Host.CreateDefaultBuilder(args)
                .ConfigureJuniperHost<Startup>(ports);
        }
    }
}
