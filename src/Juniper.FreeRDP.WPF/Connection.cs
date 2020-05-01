using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper
{
    [Serializable]
    public sealed class Connection : ISerializable
    {
        public string Name { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public string WindowSize { get; set; }
        public int ColorDepth { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool SavePassword { get; set; }
        public string Domain { get; set; }

        public Connection()
        {
            Name = "<New connection>";
            Port = 3389;
        }

        private Connection(SerializationInfo info, StreamingContext context)
        {
            Name = info.GetString(nameof(Name));
            Server = info.GetString(nameof(Server));
            Port = info.GetInt32(nameof(Port));
            WindowSize = info.GetString(nameof(WindowSize));
            ColorDepth = info.GetInt32(nameof(ColorDepth));
            UserName = info.GetString(nameof(UserName));
            Password = info.GetString(nameof(Password));
            SavePassword = info.GetBoolean(nameof(SavePassword));
            Domain = info.GetString(nameof(Domain));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info is null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            info.AddValue(nameof(Name), Name);
            info.AddValue(nameof(Server), Server);
            info.AddValue(nameof(Port), Port);
            info.AddValue(nameof(WindowSize), WindowSize);
            info.AddValue(nameof(ColorDepth), ColorDepth);
            info.AddValue(nameof(UserName), UserName);
            info.AddValue(nameof(Password), SavePassword ? Password : string.Empty);
            info.AddValue(nameof(SavePassword), SavePassword);
            info.AddValue(nameof(Domain), Domain);
        }

        public void Start()
        {
            if (!string.IsNullOrEmpty(Server))
            {
                var rdp = string.IsNullOrEmpty(Properties.Settings.Default.FreeRDPPath)
                        ? "rdp"
                        : Properties.Settings.Default.FreeRDPPath;
                var procStart = new ProcessStartInfo(rdp)
                {
                    UseShellExecute = true
                };

                procStart.ArgumentList.Add($"/v:{Server}");
                procStart.ArgumentList.Add("/gdi:hw");
                procStart.ArgumentList.Add("/themes");
                procStart.ArgumentList.Add("/aero");
                procStart.ArgumentList.Add("/decorations");
                procStart.ArgumentList.Add("/fonts");


                if (Port != 3389)
                {
                    procStart.ArgumentList.Add($"/port:{Port}");
                }

                if (WindowSize is object
                    && WindowSize != "Fullscreen")
                {
                    procStart.ArgumentList.Add($"/size:{WindowSize}");
                }
                else
                {
                    procStart.ArgumentList.Add("/f");
                }

                if (ColorDepth > 0)
                {
                    procStart.ArgumentList.Add($"/bpp:{ColorDepth}");
                }

                if (!string.IsNullOrEmpty(UserName))
                {
                    procStart.ArgumentList.Add($"/u:{UserName}");
                }

                if (!string.IsNullOrEmpty(Password))
                {
                    procStart.ArgumentList.Add($"/p:{Password}");
                }

                if (!string.IsNullOrEmpty(Domain))
                {
                    procStart.ArgumentList.Add($"/d:{Domain}");
                }

                Process.Start(procStart);
            }
        }

        private static FileInfo _connectionsFile;
        private static FileInfo ConnectionsFile
        {
            get
            {
                if (_connectionsFile is null)
                {
                    var settingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                    var settingsFilePath = Path.Combine(settingsDirectoryPath, "JFreeRDP", "connections.json");
                    _connectionsFile = new FileInfo(settingsFilePath);
                }
                return _connectionsFile;
            }
        }

        public static List<Connection> Load()
        {
            if (ConnectionsFile.Exists)
            {
                var loader = new JsonFactory<Connection[]>();
                using var stream = ConnectionsFile.OpenRead();
                if (loader.TryDeserialize(stream, out var connections))
                {
                    return connections.ToList();
                }
            }
            return new List<Connection>();
        }

        public static void Save(List<Connection> connections)
        {
            if (connections is null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            ConnectionsFile.Directory.Create();
            var saver = new JsonFactory<Connection[]>();
            using var stream = ConnectionsFile.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            saver.Serialize(stream, connections.ToArray());
        }
    }
}
