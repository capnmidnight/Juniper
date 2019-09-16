using System;
using System.IO;
using System.Runtime.Serialization;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class PackageInstallProgress : ISerializable
    {
        public readonly FileInfo PackageFile;
        public string[] paths;
        public int installedFiles;
        public PackageScanStatus progress;

        public PackageInstallProgress(FileInfo file)
        {
            PackageFile = file;
        }

        private PackageInstallProgress(SerializationInfo info, StreamingContext context)
        {
            PackageFile = new FileInfo(info.GetString("path"));
            paths = (string[])info.GetValue(nameof(paths), typeof(string[]));
            installedFiles = info.GetInt32(nameof(installedFiles));
            Enum.TryParse(info.GetString(nameof(progress)), out progress);
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("path", PackageFile.FullName);
            info.AddValue(nameof(paths), paths);
            info.AddValue(nameof(installedFiles), installedFiles);
            info.AddValue(nameof(progress), progress.ToString());
        }
    }
}
