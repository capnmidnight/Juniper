using System;
using System.Diagnostics;
using System.IO;
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
            var myPictures = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            var cacheDirName = Path.Combine(myPictures, "GoogleMaps");
            var cacheDir = new DirectoryInfo(cacheDirName);
            var keyFileName = Path.Combine(cacheDirName, "keys.txt");
            var keyFile = new FileInfo(keyFileName);
            using (var fileStream = keyFile.OpenRead())
            using (var reader = new StreamReader(fileStream))
            {
                var apiKey = reader.ReadLine();
                var signingKey = reader.ReadLine();
                server = new YarrowServer(args, Info, Warning, Error, apiKey, signingKey, cacheDir);
                server.Start();
            }
        }

        protected override void OnStop()
        {
            server?.Stop();
        }
    }
}