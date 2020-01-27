using System.Collections.Generic;
using System.IO;

using Juniper.Compression;

namespace Juniper.ConfigurationManagement
{
    public abstract class AbstractCompressedPackage : AbstractPackage2
    {
        protected AbstractCompressedPackage(string name, string version, string path)
        : base(name, version, path)
        { }

        protected int InstallPercentage()
        {
            var fileCount = 0;
            var installedCount = 0;

            foreach (var entry in GetContentFiles())
            {
                if (entry.IsFile)
                {
                    ++fileCount;
                    var filePath = Path.Combine(InstallDirectory, entry.FullName);
                    var exists = File.Exists(filePath);
                    if (exists)
                    {
                        ++installedCount;
                    }
                }
            }

            var percentInstalled = 100 * installedCount / fileCount;
            return percentInstalled;
        }

        public override bool Available => File.Exists(ContentPath);

        public override bool Cached => Available;

        public override bool IsInstalled => InstallPercentage() > 10;

        public override bool CanUpdate => InstallPercentage() < 95;

        protected abstract string InstallDirectory { get; }

        protected abstract IEnumerable<CompressedFileInfo> GetContentFiles();
    }
}
