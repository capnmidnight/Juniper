using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Juniper.IO;

using static System.Console;

namespace Juniper.ConfigurationManagement
{
    public static class Program
    {
        public static void Main()
        {
            var unityProjectDir = @"D:\Projects\Juniper\examples\Juniper - Android"; // @"C:\Users\smcbeth.DLS-INC\Projects\Yarrow\src\Yarrow - AndroidOculus";

            AbstractPackage2.UnityProjectRoot = unityProjectDir;
            var packageDB = GetPackages();


            var configFactory = new JsonFactory<UnityPlatformConfigurations>();
            var juniperPath = Path.Combine(unityProjectDir, "Assets", "Juniper");
            var juniperPlatformsFileName = Path.Combine(juniperPath, "platforms.json");
            foreach (var group in packageDB)
            {
                WriteLine(group.Key);
                foreach (var version in group.Value)
                {
                    var installed = version.IsInstalled.ToYesNo(nameof(version.IsInstalled));
                    var avail = version.Available.ToYesNo(nameof(version.Available));
                    var cached = version.Cached.ToYesNo(nameof(version.Cached));

                    WriteLine($"\t{version.CompilerDefine} {version.Version ?? "N/A"} {version.Source}");
                    WriteLine($"\t{Units.Converter.Label(version.InstallPercentage, Units.UnitOfMeasure.Percent)}");
                    WriteLine($"\t{installed} {avail}, {cached}]:> {version.ContentPath}");
                }
            }
        }


        private static IReadOnlyDictionary<string, IReadOnlyCollection<AbstractPackage2>> GetPackages()
        {
            var packages = new List<AbstractPackage2>();

            UnityAssetStorePackage.GetPackages(packages);
            JuniperZipPackage.GetPackages(packages);
            UnityPackageManagerPackage.GetPackages(packages);

            return (from package in packages
                    group package by package.PackageID into grp
                    orderby grp.Key
                    select grp)
                    .ToDictionary(
                        g => g.Key,
                        g => (IReadOnlyCollection<AbstractPackage2>)g.ToArray());
        }
    }

    public class UnityPlatformConfigurations
    {

    }
}
