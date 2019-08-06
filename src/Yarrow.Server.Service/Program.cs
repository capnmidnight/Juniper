using System.ServiceProcess;

namespace Yarrow.Server
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        private static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new YarrowServerService()
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}