using System.Diagnostics;
using System.ServiceProcess;

namespace Yarrow.Server
{
    public partial class YarrowServerService : ServiceBase
    {
        private YarrowServer server;

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
            server = new YarrowServer(args, Info, Warning, Error);
        }

        protected override void OnStop()
        {
            server?.Stop();
        }
    }
}