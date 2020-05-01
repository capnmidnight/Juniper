using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;

using Juniper.IO;

namespace Juniper
{
    [Serializable]
    public sealed class Connection : ISerializable, INotifyPropertyChanged
    {
        public static readonly string[] WindowSizes =
        {
            "Fullscreen",
            "1920x1080",
            "1280x720",
            "1152x864",
            "1024x768",
            "800x600"
        };

        public static readonly string[] ColorDepths = {
            "Default",
            "16",
            "32"
        };

        public event PropertyChangedEventHandler PropertyChanged;

        private string lastName;
        private string curName;
        private static readonly PropertyChangedEventArgs changedName = new PropertyChangedEventArgs(nameof(Name));
        public string Name
        {
            get
            {
                return curName;
            }
            set
            {
                if (curName != value)
                {
                    curName = value;
                    PropertyChanged?.Invoke(this, changedName);
                    CheckDirty();
                }
            }
        }

        private string lastServer;
        private string curServer;
        private static readonly PropertyChangedEventArgs changedServer = new PropertyChangedEventArgs(nameof(Server));
        public string Server
        {
            get
            {
                return curServer;
            }
            set
            {
                if (curServer != value)
                {
                    curServer = value;
                    PropertyChanged?.Invoke(this, changedServer);
                    CheckDirty();
                }
            }
        }

        private int lastPort;
        private int curPort;
        private static readonly PropertyChangedEventArgs changedPort = new PropertyChangedEventArgs(nameof(Port));
        public int Port
        {
            get
            {
                return curPort;
            }
            set
            {
                if (curPort != value)
                {
                    curPort = value;
                    PropertyChanged?.Invoke(this, changedPort);
                    CheckDirty();
                }
            }
        }

        private string lastWindowSize;
        private string curWindowSize;
        private static readonly PropertyChangedEventArgs changedWindowSize = new PropertyChangedEventArgs(nameof(WindowSize));
        public string WindowSize
        {
            get
            {
                return curWindowSize;
            }
            set
            {
                if (curWindowSize != value)
                {
                    curWindowSize = value;
                    PropertyChanged?.Invoke(this, changedWindowSize);
                    CheckDirty();
                }
            }
        }

        private string lastColorDepth;
        private string curColorDepth;
        private static readonly PropertyChangedEventArgs changedColorDepth = new PropertyChangedEventArgs(nameof(ColorDepth));
        public string ColorDepth
        {
            get
            {
                return curColorDepth;
            }
            set
            {
                if (curColorDepth != value)
                {
                    curColorDepth = value;
                    PropertyChanged?.Invoke(this, changedColorDepth);
                    CheckDirty();
                }
            }
        }

        private string lastUserName;
        private string curUserName;
        private static readonly PropertyChangedEventArgs changedUserName = new PropertyChangedEventArgs(nameof(UserName));
        public string UserName
        {
            get
            {
                return curUserName;
            }
            set
            {
                if (curUserName != value)
                {
                    curUserName = value;
                    PropertyChanged?.Invoke(this, changedUserName);
                    CheckDirty();
                }
            }
        }


        private string lastDomain;
        private string curDomain;
        private static readonly PropertyChangedEventArgs changedDomain = new PropertyChangedEventArgs(nameof(Domain));
        public string Domain
        {
            get
            {
                return curDomain;
            }
            set
            {
                if (curDomain != value)
                {
                    curDomain = value;
                    PropertyChanged?.Invoke(this, changedDomain);
                    CheckDirty();
                }
            }
        }

        public Connection()
        {
            curName = "<New connection>";
            curPort = 3389;
            curWindowSize = WindowSizes[0];
            curColorDepth = ColorDepths[0];
            Commit();
        }

        private Connection(SerializationInfo info, StreamingContext context)
        {
            curName = info.GetString(nameof(Name));
            curServer = info.GetString(nameof(Server));
            curPort = info.GetInt32(nameof(Port));
            curWindowSize = info.GetString(nameof(WindowSize));
            curColorDepth = info.GetString(nameof(ColorDepth));
            curUserName = info.GetString(nameof(UserName));
            curDomain = info.GetString(nameof(Domain));
            Commit();
        }

        private bool wasDirty;
        public bool IsDirty
        {
            get
            {
                return Name != lastName
                    || Server != lastServer
                    || Port != lastPort
                    || WindowSize != lastWindowSize
                    || ColorDepth != lastColorDepth
                    || UserName != lastUserName
                    || Domain != lastDomain;
            }
        }

        private static readonly PropertyChangedEventArgs changedIsDirty = new PropertyChangedEventArgs(nameof(IsDirty));
        private void CheckDirty()
        {
            if (wasDirty != IsDirty)
            {
                wasDirty = IsDirty;
                PropertyChanged?.Invoke(this, changedIsDirty);
            }
        }

        private void Commit()
        {
            lastName = curName;
            lastServer = curServer;
            lastPort = curPort;
            lastWindowSize = curWindowSize;
            lastColorDepth = curColorDepth;
            lastUserName = curUserName;
            lastDomain = curDomain;
            CheckDirty();
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
            info.AddValue(nameof(Domain), Domain);
        }

        public void Start(string password)
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
                    && WindowSize != WindowSizes[0])
                {
                    procStart.ArgumentList.Add($"/size:{WindowSize}");
                }
                else
                {
                    procStart.ArgumentList.Add("/f");
                }

                if (ColorDepth is object
                    && ColorDepth != ColorDepths[0])
                {
                    procStart.ArgumentList.Add($"/bpp:{ColorDepth}");
                }

                if (!string.IsNullOrEmpty(UserName))
                {
                    procStart.ArgumentList.Add($"/u:{UserName}");
                }

                if (!string.IsNullOrEmpty(password))
                {
                    procStart.ArgumentList.Add($"/p:{password}");
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

        public static IEnumerable<Connection> Load()
        {
            if (ConnectionsFile.Exists)
            {
                var loader = new JsonFactory<Connection[]>();
                using var stream = ConnectionsFile.OpenRead();
                if (loader.TryDeserialize(stream, out var connections))
                {
                    return connections;
                }
            }
            return Array.Empty<Connection>();
        }

        public static void Save(IEnumerable<Connection> connections)
        {
            if (connections is null)
            {
                throw new ArgumentNullException(nameof(connections));
            }

            ConnectionsFile.Directory.Create();
            var saver = new JsonFactory<Connection[]>();
            using var stream = ConnectionsFile.Open(FileMode.Create, FileAccess.Write, FileShare.None);
            saver.Serialize(stream, connections.ToArray());
            foreach(var connection in connections)
            {
                connection.Commit();
            }
        }
    }
}
