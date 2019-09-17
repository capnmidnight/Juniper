using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using Juniper.Collections;
using Juniper.Compression;

namespace Juniper.ConfigurationManagement
{
    [Serializable]
    public sealed class PackageInstallProgress : ISerializable
    {
        public readonly FileInfo PackageFile;
        public NAryTree<CompressedFileInfo> tree;
        public int installedFiles;
        public PackageScanStatus progress;

        public PackageInstallProgress(FileInfo file)
        {
            PackageFile = file;
        }

        private PackageInstallProgress(SerializationInfo info, StreamingContext context)
        {
            PackageFile = new FileInfo(info.GetString("path"));
            installedFiles = info.GetInt32(nameof(installedFiles));
            Enum.TryParse(info.GetString(nameof(progress)), out progress);
            foreach(var entry in info)
            {
                if(entry.Name == nameof(tree))
                {
                    var paths = (CompressedFileInfo[])info.GetValue(nameof(tree), typeof(CompressedFileInfo[]));
                    tree = paths.Tree();
                    break;
                }
            }
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("path", PackageFile.FullName);
            info.AddValue(nameof(installedFiles), installedFiles);
            info.AddValue(nameof(progress), progress.ToString());
            if (tree != null)
            {
                info.AddValue(nameof(tree), tree.Flatten().Select(t => t.Value).ToArray());
            }
        }
    }
}
