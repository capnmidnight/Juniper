using System;
using System.Collections.Generic;
using System.Linq;

namespace Juniper.ConfigurationManagement
{
    public abstract class AbstractPackage :
        PackageReference
    {
        public static IReadOnlyDictionary<string, IReadOnlyCollection<AbstractPackage>> Load()
        {
            var packages = new List<AbstractPackage>();

            AssetStorePackage.Load(packages);
            JuniperPackage.Load(packages);
            UnityPackage.Load(packages);

            return (from package in packages
                    group package by package.PackageID into grp
                    orderby grp.Key
                    select grp)
                    .ToDictionary(
                        g => g.Key,
                        g => (IReadOnlyCollection<AbstractPackage>)g.ToArray());
        }

        public string Name { get; }

        public string ContentPath { get; }

        public string CompilerDefine { get; }

        private static string MakePackageSpec(PackageSources source, string packageID, string version)
        {
            if (packageID is null)
            {
                throw new ArgumentNullException(nameof(packageID));
            }

            if (source == PackageSources.UnityPackageManager && version is null)
            {
                throw new ArgumentNullException(nameof(version));
            }

            return source == PackageSources.UnityPackageManager
                ? FormatUnityPackageManagerSpec(packageID, version)
                : packageID;
        }

        protected AbstractPackage(PackageSources source, string packageID, string name, string version, string path, string compilerDefine)
            : base(source, MakePackageSpec(source, packageID, version))
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            ContentPath = path;
            CompilerDefine = compilerDefine ?? throw new ArgumentNullException(nameof(compilerDefine));
        }

        public override string ToString()
        {
            var nameString = base.ToString();

            if (ContentPath is null)
            {
                return $"{nameString} [{CompilerDefine}] (builtin)";
            }
            else
            {
                return $"{nameString} [{CompilerDefine}] at {ContentPath}";
            }
        }

        public abstract bool Available { get; }

        public abstract bool Cached { get; }

        public abstract float InstallPercentage { get; }

        public abstract bool IsInstalled { get; }

        public abstract bool CanUpdate { get; }

        public abstract void Install();

        public override int GetHashCode()
        {
            var hashCode = -1326761686;
            var comparer = EqualityComparer<string>.Default;
            hashCode = (hashCode * -1521134295) + comparer.GetHashCode(Name);
            hashCode = (hashCode * -1521134295) + comparer.GetHashCode(VersionSpec);
            hashCode = (hashCode * -1521134295) + comparer.GetHashCode(CompilerDefine);
            hashCode = (hashCode * -1521134295) + comparer.GetHashCode(PackageID);
            hashCode = (hashCode * -1521134295) + Source.GetHashCode();
            return hashCode;
        }

        public virtual void Activate()
        {
            Project.AddCompilerDefine(CompilerDefine);
        }
    }
}
