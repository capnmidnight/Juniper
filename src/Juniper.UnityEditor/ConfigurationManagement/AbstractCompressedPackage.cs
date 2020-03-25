using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Juniper.Compression;
using Juniper.IO;

namespace Juniper.ConfigurationManagement
{
    public abstract class AbstractCompressedPackage : AbstractPackage
    {

        private static Dictionary<string, string> compilerDefines;

        private static IReadOnlyDictionary<string, string> CompilerDefineSymbols
        {
            get
            {
                if (compilerDefines is null)
                {
                    using var stream = FileDataSource.Instance.GetStream(Project.JuniperDefinesFileName);
                    var definesFactory = new JsonFactory<PackageDefineSymbol[]>();
                    compilerDefines = definesFactory.Deserialize(stream)
                        .ToDictionary(d => d.Name, d => d.CompilerDefine);
                }

                return compilerDefines;
            }
        }

        private static readonly Regex NameNormalizer = new Regex("\\W+", RegexOptions.Compiled);
        private static readonly Regex SpaceNormalizer = new Regex("_{2,}", RegexOptions.Compiled);

        private static string GetCompilerDefineSymbol(string name)
        {
            if (CompilerDefineSymbols.ContainsKey(name))
            {
                return CompilerDefineSymbols[name];
            }
            else
            {
                return SpaceNormalizer.Replace(
                    NameNormalizer.Replace(
                        name,
                        "_"),
                    "_");
            }
        }

        protected AbstractCompressedPackage(PackageSources source, string name, string version, string path)
        : base(source, name, name, version, path ?? throw new ArgumentNullException(nameof(path)), GetCompilerDefineSymbol(name))
        { }

        private Task scanningTask;

        private float installPercent;

        public override float InstallPercentage
        {
            get
            {
                if (!scanningTask.IsRunning())
                {
                    scanningTask = Task.Run(Scan);
                }

                return installPercent;
            }
        }

        private void Scan()
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

            installPercent = Units.Ratio.Percent(installedCount, fileCount);
        }

        public override bool Available => File.Exists(ContentPath);

        public override bool Cached => Available;

        public override bool IsInstalled => InstallPercentage > 10;

        public override bool CanUpdate => InstallPercentage < 95;

        protected abstract string InstallDirectory { get; }

        protected abstract IEnumerable<CompressedFileInfo> GetContentFiles();
    }
}
