using System.Diagnostics;
using System.ServiceProcess;

using Juniper.HTTP;

namespace Yarrow.Server
{
    public partial class YarrowServerService : ServiceBase
    {
        private HttpServer server;

        public YarrowServerService()
        {
            InitializeComponent();
        }

        private void Error(string message)
        {
            EventLog.WriteEntry(message, EventLogEntryType.Error);
            Stop();
        }

        private void Warning(string message)
        {
            EventLog.WriteEntry(message, EventLogEntryType.Warning);
        }

        private void Info(string message)
        {
            EventLog.WriteEntry(message, EventLogEntryType.Information);
        }

        protected override void OnStart(string[] args)
        {
            HttpServerUtil.Start(args, Info, Warning, Error);
        }

        protected override void OnStop()
        {
            if (server != null)
            {
                server.Stop();
            }
        }
    }
}